using System.Globalization;
using System.Text;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IMercadoPagoOrdersClient _ordersClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ISubscriptionRepository subscriptionRepository,
            IPlanRepository planRepository,
            IMercadoPagoOrdersClient ordersClient,
            IConfiguration configuration,
            ILogger<PaymentService> logger
        )
        {
            _paymentRepository = paymentRepository;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _ordersClient = ordersClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<PaymentInitiatedDto> InitiateAsync(CreatePaymentDto dto, Guid adminId,
            CancellationToken cancellationToken)
        {
            var plan = await _planRepository.GetByIdAsync(dto.PlanId, cancellationToken)
                       ?? throw new NotFoundException("Plano", dto.PlanId);

            var paymentId = Guid.NewGuid();

            var successUrl = _configuration["MercadoPago:BackUrls:Success"];
            var failureUrl = _configuration["MercadoPago:BackUrls:Failure"];
            var pendingUrl = _configuration["MercadoPago:BackUrls:Pending"];
            if (string.IsNullOrEmpty(successUrl))
                throw new InvalidOperationException("MercadoPago:BackUrls:Success não configurado no appsettings.");

            // O valor SEMPRE vem do plano no banco, nunca do front. Orders exige string com ponto e 2 casas.
            var amount = plan.Price.ToString("F2", CultureInfo.InvariantCulture);

            var orderRequest = new MpCreateOrderRequest
            {
                TotalAmount = amount,
                ExternalReference = paymentId.ToString(),
                Items =
                [
                    new MpOrderItem
                    {
                        Title = plan.Name,
                        UnitPrice = amount,
                        Quantity = 1
                    }
                ],
                Config = new MpOrderConfig
                {
                    Online = new MpOnlineConfig
                    {
                        SuccessUrl = successUrl,
                        FailureUrl = failureUrl,
                        PendingUrl = pendingUrl,
                        AutoReturn = "approved"
                    }
                }
            };

            // paymentId como idempotency key: um retry da mesma tentativa não cria order duplicada.
            var order = await _ordersClient.CreateAsync(orderRequest, paymentId.ToString(), cancellationToken);

            if (string.IsNullOrEmpty(order.Id) || string.IsNullOrEmpty(order.CheckoutUrl))
                throw new InvalidOperationException("Mercado Pago não retornou id/checkout_url da order.");

            var payment = new Payment
            {
                Id = paymentId,
                Amount = plan.Price,
                // Method fica null aqui: o comprador escolhe no checkout e preenchemos via webhook.
                Status = PaymentStatus.Pending,
                Date = DateTime.UtcNow,
                AdminId = adminId,
                PlanId = dto.PlanId,
                MercadoPagoOrderId = order.Id
            };

            await _paymentRepository.AddAsync(payment); // único SaveChangesAsync

            _logger.LogInformation(
                "Pagamento {PaymentId} iniciado para Admin {AdminId}, Plano {PlanId}, Order {OrderId}",
                payment.Id, adminId, dto.PlanId, order.Id);

            return new PaymentInitiatedDto(
                PaymentId: payment.Id,
                CheckoutUrl: order.CheckoutUrl
            );
        }

        public async Task HandleWebhookAsync(MercadoPagoWebhookDto webhook, string? signatureHeader,
            string? requestIdHeader, CancellationToken cancellationToken)
        {
            if (!IsOrderEvent(webhook))
            {
                _logger.LogInformation("Webhook ignorado — type: {Type}, action: {Action}", webhook.Type,
                    webhook.Action);
                return;
            }

            var orderId = webhook.Data?.Id;
            if (string.IsNullOrWhiteSpace(orderId))
            {
                _logger.LogWarning("Webhook de order sem data.id. Ignorando.");
                return;
            }

            if (!IsValidSignature(orderId, signatureHeader, requestIdHeader))
            {
                _logger.LogWarning("Assinatura inválida no webhook — possível requisição forjada. Ignorando.");
                return;
            }

            // A notificação só avisa "algo mudou". A fonte da verdade é o GET da order.
            // Se falhar (exceção), o controller devolve 500 e o Mercado Pago reenvia depois.
            var order = await _ordersClient.GetAsync(orderId, cancellationToken);

            if (!Guid.TryParse(order.ExternalReference, out var internalPaymentId))
            {
                _logger.LogWarning("Order {OrderId} com ExternalReference inválida: {Ref}", orderId,
                    order.ExternalReference);
                return;
            }

            var payment = await _paymentRepository.GetByIdAsync(internalPaymentId, cancellationToken);
            if (payment is null)
            {
                _logger.LogWarning("Payment interno {Id} não encontrado.", internalPaymentId);
                return;
            }

            // Garante que a order consultada é a mesma que criamos para esse Payment.
            if (!string.Equals(payment.MercadoPagoOrderId, order.Id, StringComparison.Ordinal))
            {
                _logger.LogWarning("Order {OrderId} não corresponde ao Payment {Id}. Ignorando.", order.Id,
                    payment.Id);
                return;
            }

            var newStatus = MapStatus(order.Status, order.StatusDetail);
            if (newStatus is null)
            {
                _logger.LogInformation(
                    "Order {OrderId} com status sem ação — status: {Status}, detail: {Detail}",
                    orderId, order.Status, order.StatusDetail);
                return;
            }

            // Webhooks podem chegar fora de ordem: um pagamento confirmado não volta atrás.
            if (payment.Status == PaymentStatus.Confirmed && newStatus != PaymentStatus.Confirmed)
            {
                _logger.LogInformation("Webhook fora de ordem ignorado — Payment {Id} já Confirmed.", payment.Id);
                return;
            }

            if (payment.Status == newStatus)
            {
                _logger.LogInformation(
                    "Webhook duplicado ignorado — Payment {Id} já está {Status}", payment.Id, payment.Status);
                return;
            }

            // Opção B: o método só é conhecido depois que o comprador paga/escolhe no checkout.
            var mpPayment = order.Transactions?.Payments?.FirstOrDefault();
            var method = mpPayment?.PaymentMethod?.Id ?? mpPayment?.PaymentMethod?.Type;
            if (!string.IsNullOrEmpty(method)) payment.Method = method;
            if (!string.IsNullOrEmpty(mpPayment?.Id)) payment.MercadoPagoPaymentId = mpPayment.Id;

            payment.Status = newStatus.Value;
            await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation("Payment {Id} atualizado para {Status}", payment.Id, payment.Status);

            if (newStatus == PaymentStatus.Confirmed)
                await HandleApprovedPaymentAsync(payment, cancellationToken);
            else if (newStatus == PaymentStatus.Failed)
                await HandleFailedPaymentAsync(payment, cancellationToken);
        }

        // Status da order (doc "Status da order"):
        //   processed + accredited          -> aprovado
        //   failed | canceled               -> falhou (canceled inclui order expirada)
        //   created | processing | action_required -> ainda pendente
        //   processed + refunded/partially_refunded, refunded -> sem ação (seu enum não tem Refunded)
        private static PaymentStatus? MapStatus(string? status, string? detail) => (status, detail) switch
        {
            ("processed", "accredited") => PaymentStatus.Confirmed,
            ("failed" or "canceled", _) => PaymentStatus.Failed,
            ("created" or "processing" or "action_required", _) => PaymentStatus.Pending,
            _ => null
        };

        // A doc mostra type "order" no corpo, mas a página de status cita o tópico "orders_v2".
        // Aceito os dois e, na dúvida, confio no prefixo da action.
        private static bool IsOrderEvent(MercadoPagoWebhookDto webhook)
            => webhook.Type is "order" or "orders_v2"
               || (webhook.Action?.StartsWith("order", StringComparison.OrdinalIgnoreCase) ?? false);

        private async Task HandleApprovedPaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            var byPayment = await _subscriptionRepository.GetByPaymentIdAsync(payment.Id, cancellationToken);
            if (byPayment is not null)
            {
                if (!byPayment.IsActive)
                {
                    byPayment.IsActive = true;
                    await _subscriptionRepository.UpdateAsync(byPayment);
                }

                return;
            }

            var plan = await _planRepository.GetByIdAsync(payment.PlanId, cancellationToken)
                       ?? throw new NotFoundException("Plano", payment.PlanId);

            var active = await _subscriptionRepository.GetActiveByAdminIdAsync(payment.AdminId, cancellationToken);

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

                    await _subscriptionRepository.AddAsync(subscription, cancellationToken);
                }
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _logger.LogInformation(
                    "Corrida detectada no webhook — Payment {Id} já processado por outra execução.", payment.Id);
            }
        }

        public async Task<IEnumerable<PaymentHistoryDto>> GetHistoryByAdminAsync(Guid adminId,
            CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetByAdminIdAsync(adminId, cancellationToken);

            return payments.Select(p => new PaymentHistoryDto(
                Id: p.Id,
                Amount: p.Amount,
                Date: p.Date,
                Method: p.Method,
                Status: p.Status.ToString(),
                MercadoPagoPaymentId: p.MercadoPagoPaymentId
            ));
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
            => ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
               && (sqlEx.Number == 2601 || sqlEx.Number == 2627);

        private async Task HandleFailedPaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionRepository.GetByPaymentIdAsync(payment.Id, cancellationToken);
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
                // Falha fechada por padrão. Só pula a validação se você pedir explicitamente (ex.: dev local).
                if (_configuration.GetValue<bool>("MercadoPago:SkipSignatureValidation"))
                {
                    _logger.LogWarning("MercadoPago:WebhookSecret ausente — validação pulada por configuração.");
                    return true;
                }

                _logger.LogError("MercadoPago:WebhookSecret não configurado. Webhook rejeitado.");
                return false;
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

            // IDs de order são alfanuméricos (ORD01...), então entram em minúsculas no manifesto.
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