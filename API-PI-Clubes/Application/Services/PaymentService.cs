using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<PaymentInitiatedDto> InitiateAsync(CreatePaymentDto dto)
        {
            var plan = await _planRepository.GetByIdAsync(dto.PlanId)
                ?? throw new Exception("Plano não encontrado.");
     
            // Cria o Payment pendente
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                Amount = plan.Price,
                Method = dto.Method,
                Status = PaymentStatus.Pending,
                Date = DateTime.UtcNow,
                AdminId = dto.AdminId,   // ← obrigatório
                PlanId = dto.PlanId      // ← obrigatório
            };

            await _paymentRepository.AddAsync(payment);

            var successUrl = _configuration["MercadoPago:BackUrls:Success"];
            var failureUrl = _configuration["MercadoPago:BackUrls:Failure"];
            var pendingUrl = _configuration["MercadoPago:BackUrls:Pending"];
            if (string.IsNullOrEmpty(successUrl))
                throw new InvalidOperationException(
                    "MercadoPago:BackUrls:Success não configurado no appsettings.");

            // Cria a preference no Mercado Pago
            MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"]!;
            
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
     
                ExternalReference = payment.Id.ToString()
            };
     
            var client = new PreferenceClient();
            var preference = await client.CreateAsync(preferenceRequest);
            payment.GatewayTransactionId = preference.Id;
            await _paymentRepository.UpdateAsync(payment);
            
            _logger.LogInformation(
                "Pagamento {PaymentId} iniciado para Admin {AdminId}, Plano {PlanId}",
                payment.Id, dto.AdminId, dto.PlanId);
     
            return new PaymentInitiatedDto(
                PaymentId: payment.Id,
                CheckoutUrl: preference.InitPoint
            );
        }
     
        public async Task HandleWebhookAsync(MercadoPagoWebhookDto webhook)
        {
            // Filtra ações relevantes — loga as desconhecidas para diagnóstico
            if (webhook.Action is not ("payment.updated" or "payment.created" or "payment" or ""))
            {
                _logger.LogInformation("Webhook ignorado — action: {Action}", webhook.Action);
                return;
            }
     
            // CORREÇÃO: TryParse em vez de Parse direto
            if (!long.TryParse(webhook.Data?.Id, out var mpPaymentId))
            {
                _logger.LogWarning("Webhook com ID inválido: {Id}", webhook.Data?.Id);
                return;
            }
     
            // Busca o pagamento real no Mercado Pago
            MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"]!;
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
     
            // CORREÇÃO: Idempotência — ignora se já foi processado
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
     
            // Atualiza o Payment
            payment.Status = newStatus;
            payment.GatewayTransactionId = mpPayment.Id.ToString();
            await _paymentRepository.UpdateAsync(payment);
     
            _logger.LogInformation(
                "Payment {Id} atualizado para {Status}", payment.Id, payment.Status);
     
            // Só age na subscription se foi aprovado ou recusado
            if (newStatus == PaymentStatus.Confirmed)
                await HandleApprovedPaymentAsync(payment, mpPayment.ExternalReference);
            else if (newStatus == PaymentStatus.Failed)
                await HandleFailedPaymentAsync(payment);
        }
     
        private async Task HandleApprovedPaymentAsync(Payment payment, string externalReference)
        {
            // Verifica se já existe subscription para esse payment (idempotência)
            var existing = await _subscriptionRepository.GetByPaymentIdAsync(payment.Id);
     
            if (existing is not null)
            {
                // Já existe — só garante que está ativa
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    await _subscriptionRepository.UpdateAsync(existing);
                }
                return;
            }
     
            var plan = await _planRepository.GetByIdAsync(payment.PlanId)
                ?? throw new Exception($"Plano {payment.PlanId} não encontrado.");
     
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
     
            _logger.LogInformation(
                "Subscription {SubId} criada para Admin {AdminId}, expira em {Expires}",
                subscription.Id, subscription.AdminId, subscription.ExpiresAt);
        }
     
        private async Task HandleFailedPaymentAsync(Payment payment)
        {
            var subscription = await _subscriptionRepository.GetByPaymentIdAsync(payment.Id);
     
            if (subscription is null) return;
     
            subscription.IsActive = false;
            await _subscriptionRepository.UpdateAsync(subscription);
     
            _logger.LogInformation(
                "Subscription {SubId} desativada por falha no pagamento.", subscription.Id);
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
                GatewayTransactionId: p.GatewayTransactionId
            ));
        }



    }
}
