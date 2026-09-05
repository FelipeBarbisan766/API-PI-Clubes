using System.Text;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using API_PI_Clubes.Application.Exceptions;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ISubscriptionRepository subscriptionRepository,
            IPlanRepository planRepository,
            IConfiguration configuration,
            ILogger<PaymentService> logger
        )
        {
            _paymentRepository = paymentRepository;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<PaymentInitiatedDto> InitiateAsync(CreatePaymentDto dto, Guid adminId)
        {
            var plan = await _planRepository.GetByIdAsync(dto.PlanId)
                       ?? throw new NotFoundException("Plano", dto.PlanId);

            var paymentId = Guid.NewGuid();

            var successUrl = _configuration["MercadoPago:BackUrls:Success"];
            var failureUrl = _configuration["MercadoPago:BackUrls:Failure"];
            var pendingUrl = _configuration["MercadoPago:BackUrls:Pending"];
            if (string.IsNullOrEmpty(successUrl))
                throw new InvalidOperationException("MercadoPago:BackUrls:Success não configurado no appsettings.");

            var preferenceRequest = new PreferenceRequest
            {
                Items =
                [
                    new PreferenceItemRequest
                    {
                        Id = plan.Id.ToString(),
                        Title = plan.Name,
                        Description = plan.Description,
                        Quantity = 1,
                        CurrencyId = "BRL",
                        UnitPrice = plan.Price
                    }
                ],
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = successUrl,
                    Failure = failureUrl,
                    Pending = pendingUrl
                },
                AutoReturn = "approved",
                NotificationUrl = _configuration["MercadoPago:WebhookUrl"],
                ExternalReference = paymentId.ToString()
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(preferenceRequest);

            var payment = new Payment
            {
                Id = paymentId,
                Amount = plan.Price,
                Method = dto.Method,
                Status = PaymentStatus.Pending,
                Date = DateTime.UtcNow,
                AdminId = adminId,
                PlanId = dto.PlanId,
                MercadoPagoPreferenceId = preference.Id
            };

            await _paymentRepository.AddAsync(payment); // único SaveChangesAsync

            _logger.LogInformation(
                "Pagamento {PaymentId} iniciado para Admin {AdminId}, Plano {PlanId}",
                payment.Id, adminId, dto.PlanId);

            return new PaymentInitiatedDto(
                PaymentId: payment.Id,
                CheckoutUrl: preference.InitPoint
            );
        }

        public async Task HandleWebhookAsync(MercadoPagoWebhookDto webhook, string? signatureHeader,
            string? requestIdHeader)
        {
            if (webhook.Action is not ("payment.updated" or "payment.created" or "payment" or ""))
            {
                _logger.LogInformation("Webhook ignorado — action: {Action}", webhook.Action);
                return;
            }

            if (!long.TryParse(webhook.Data?.Id, out var mpPaymentId))
            {
                _logger.LogWarning("Webhook com ID inválido: {Id}", webhook.Data?.Id);
                return;
            }

            if (!IsValidSignature(webhook.Data!.Id, signatureHeader, requestIdHeader))
            {
                _logger.LogWarning("Assinatura inválida no webhook — possível requisição forjada. Ignorando.");
                return;
            }

            var mpClient = new MercadoPago.Client.Payment.PaymentClient();
            var mpPayment = await mpClient.GetAsync(mpPaymentId);

            if (mpPayment?.ExternalReference is null)
            {
                _logger.LogWarning("Pagamento {MpId} sem ExternalReference. Ignorando.", mpPaymentId);
                return;
            }

            if (!Guid.TryParse(mpPayment.ExternalReference, out var internalPaymentId))
            {
                _logger.LogWarning("ExternalReference inválida: {Ref}", mpPayment.ExternalReference);
                return;
            }

            var payment = await _paymentRepository.GetByIdAsync(internalPaymentId);

            if (payment is null)
            {
                _logger.LogWarning("Payment interno {Id} não encontrado.", internalPaymentId);
                return;
            }

            var newStatus = mpPayment.Status switch
            {
                "approved" => PaymentStatus.Confirmed,
                "rejected" or "cancelled" => PaymentStatus.Failed,
                _ => PaymentStatus.Pending
            };

            if (payment.Status == newStatus)
            {
                _logger.LogInformation(
                    "Webhook duplicado ignorado — Payment {Id} já está {Status}",
                    payment.Id, payment.Status);
                return;
            }

            payment.Status = newStatus;
            payment.MercadoPagoPaymentId = mpPayment.Id.ToString();
            await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation("Payment {Id} atualizado para {Status}", payment.Id, payment.Status);

            if (newStatus == PaymentStatus.Confirmed)
                await HandleApprovedPaymentAsync(payment);
            else if (newStatus == PaymentStatus.Failed)
                await HandleFailedPaymentAsync(payment);
        }

        private async Task HandleApprovedPaymentAsync(Payment payment)
        {
            var byPayment = await _subscriptionRepository.GetByPaymentIdAsync(payment.Id);
            if (byPayment is not null)
            {
                if (!byPayment.IsActive)
                {
                    byPayment.IsActive = true;
                    await _subscriptionRepository.UpdateAsync(byPayment);
                }

                return;
            }

            var plan = await _planRepository.GetByIdAsync(payment.PlanId)
                       ?? throw new NotFoundException("Plano", payment.PlanId);

            var active = await _subscriptionRepository.GetActiveByAdminIdAsync(payment.AdminId);

            try
            {
                if (active is not null)
                {
                    var baseDate = active.ExpiresAt > DateTime.UtcNow ? active.ExpiresAt : DateTime.UtcNow;
                    active.PaymentId = payment.Id;
                    active.PlanId = payment.PlanId;
                    active.ExpiresAt = baseDate.AddDays(plan.DurationDays);
                    active.IsActive = true;

                    await _subscriptionRepository.UpdateAsync(active);
                }
                else
                {
                    var subscription = new Subscription
                    {
                        Id = Guid.NewGuid(),
                        AdminId = payment.AdminId,
                        PlanId = payment.PlanId,
                        PaymentId = payment.Id,
                        StartDate = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(plan.DurationDays),
                        IsActive = true
                    };

                    await _subscriptionRepository.AddAsync(subscription);
                }
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _logger.LogInformation(
                    "Corrida detectada no webhook — Payment {Id} já processado por outra execução.", payment.Id);
            }
        }

        public async Task<IEnumerable<PaymentHistoryDto>> GetHistoryByAdminAsync(Guid adminId)
        {
            var payments = await _paymentRepository.GetByAdminIdAsync(adminId);

            return payments.Select(p => new PaymentHistoryDto(
                Id: p.Id,
                Amount: p.Amount,
                Date: p.Date,
                Method: p.Method.ToString(),
                Status: p.Status.ToString(),
                MercadoPagoPaymentId: p.MercadoPagoPaymentId
            ));
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
            => ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
               && (sqlEx.Number == 2601 || sqlEx.Number == 2627);

        private async Task HandleFailedPaymentAsync(Payment payment)
        {
            var subscription = await _subscriptionRepository.GetByPaymentIdAsync(payment.Id);
            if (subscription is null) return;

            subscription.IsActive = false;
            await _subscriptionRepository.UpdateAsync(subscription);

            _logger.LogInformation("Subscription {SubId} desativada por falha no pagamento.", subscription.Id);
        }

        private bool IsValidSignature(string dataId, string? signatureHeader, string? requestIdHeader)
        {
            var secret = _configuration["MercadoPago:WebhookSecret"];
            if (string.IsNullOrEmpty(secret))
            {
                _logger.LogWarning("MercadoPago:WebhookSecret não configurado — pulando validação.");
                return true; // dev sem secret configurado; não bloqueia localmente
            }

            if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(requestIdHeader))
                return false;

            var parts = signatureHeader
                .Split(',')
                .Select(p => p.Split('=', 2))
                .Where(p => p.Length == 2)
                .ToDictionary(p => p[0].Trim(), p => p[1].Trim());

            if (!parts.TryGetValue("ts", out var ts) || !parts.TryGetValue("v1", out var expectedHash))
                return false;

            var manifest = $"id:{dataId.ToLowerInvariant()};request-id:{requestIdHeader};ts:{ts};";

            using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var computedHash = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(manifest)))
                .ToLowerInvariant();

            return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedHash),
                Encoding.UTF8.GetBytes(expectedHash));
        }
    }
}