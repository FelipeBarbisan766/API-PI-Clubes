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

        public PaymentService(
            IPaymentRepository paymentRepository,
            ISubscriptionRepository subscriptionRepository,
            IPlanRepository planRepository,
            IConfiguration configuration
            )
        {
            _paymentRepository = paymentRepository;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;   
            _configuration = configuration;
        }

    public async Task<PaymentInitiatedDto> InitiateAsync(CreatePaymentDto dto)
    {
        var plan = await _planRepository.GetByIdAsync(dto.PlanId)
            ?? throw new Exception("Plano não encontrado.");
 
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = plan.Price,
            Method = dto.Method,
            Status = PaymentStatus.Pending,
            Date = DateTime.UtcNow
        };
 
        await _paymentRepository.AddAsync(payment);
 
        MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"]!;
 
        var paymentMethodsConfig = new PreferencePaymentMethodsRequest();
        var methodString = dto.Method.ToString();
        if (methodString == "Pix")
        {
            // paymentMethodsConfig.DefaultPaymentMethodId = "pix";
            paymentMethodsConfig.ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
            {
                new PreferencePaymentTypeRequest { Id = "credit_card" },
                new PreferencePaymentTypeRequest { Id = "debit_card" },
                new PreferencePaymentTypeRequest { Id = "ticket" }
            };
        }
        else if (methodString == "Boleto")
        {
            paymentMethodsConfig.ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
            {
                new PreferencePaymentTypeRequest { Id = "credit_card" },
                new PreferencePaymentTypeRequest { Id = "debit_card" },
                new PreferencePaymentTypeRequest { Id = "bank_transfer" } 
            };
        }
        else if (methodString == "CreditCard")
        {
            paymentMethodsConfig.ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
            {
                new PreferencePaymentTypeRequest { Id = "ticket" },
                new PreferencePaymentTypeRequest { Id = "bank_transfer" }
            };
        }
        var preferenceRequest = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
            {
                
                new PreferenceItemRequest
                {
                    Id = plan.Id.ToString(),
                    Title = plan.Name,
                    Description = plan.Description,
                    Quantity = 1,
                    CurrencyId = "BRL",
                    UnitPrice = plan.Price
                }
            },
            
            PaymentMethods = paymentMethodsConfig,

            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = _configuration["MercadoPago:BackUrls:Success"],
                Failure = _configuration["MercadoPago:BackUrls:Failure"],
                Pending = _configuration["MercadoPago:BackUrls:Pending"]
            },
            AutoReturn = "approved",
 
            NotificationUrl = _configuration["MercadoPago:WebhookUrl"],
 
            ExternalReference = payment.Id.ToString()
        };
 
        var client = new PreferenceClient();
        Preference preference = await client.CreateAsync(preferenceRequest);
 
        payment.GatewayTransactionId = preference.Id;
        await _paymentRepository.UpdateAsync(payment);
 
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            AdminId = dto.AdminId,
            PlanId = dto.PlanId,
            PaymentId = payment.Id,
            StartDate = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(plan.DurationDays),
            IsActive = false  
        };
 
        await _subscriptionRepository.AddAsync(subscription);
 
        return new PaymentInitiatedDto(
            PaymentId: payment.Id,
            CheckoutUrl: preference.InitPoint  
        );
    }
 
    public async Task HandleWebhookAsync(MercadoPagoWebhookDto webhook)
    {
        // Só processa eventos de pagamento relevantes
        if (webhook.Action is not ("payment.updated" or "payment.created"))
            return;
 
        // Busca o pagamento real no Mercado Pago pelo ID recebido no webhook
        MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"]!;
        var mpPaymentClient = new MercadoPago.Client.Payment.PaymentClient();
        var mpPayment = await mpPaymentClient.GetAsync(long.Parse(webhook.Data.Id));
 
        if (mpPayment?.ExternalReference is null)
            return;
 
        // Acha o Payment interno pela ExternalReference
        var internalPaymentId = Guid.Parse(mpPayment.ExternalReference);
        var payment = await _paymentRepository.GetByIdAsync(internalPaymentId);
 
        if (payment is null)
            return;
 
        // Atualiza o status conforme retorno do MP
        payment.Status = mpPayment.Status switch
        {
            "approved" => PaymentStatus.Confirmed,
            "rejected" or "cancelled" => PaymentStatus.Failed,
            _ => PaymentStatus.Pending
        };
 
        // Salva o ID da transação real (não mais o PreferenceId)
        payment.GatewayTransactionId = mpPayment.Id.ToString();
        await _paymentRepository.UpdateAsync(payment);
 
        // Ativa ou desativa a Subscription conforme o resultado
        var subscription = await _subscriptionRepository.GetByPaymentIdAsync(payment.Id);
 
        if (subscription is null)
            return;
 
        subscription.IsActive = payment.Status == PaymentStatus.Confirmed;
        await _subscriptionRepository.UpdateAsync(subscription);
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
