using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public record CreatePaymentDto(
        Guid AdminId,
        Guid PlanId,
        PaymentMethod Method
    );
 
    public record PaymentInitiatedDto(
        Guid PaymentId,
        string CheckoutUrl 
    );
 
    public record MercadoPagoWebhookDto(
        string Action,   
        MercadoPagoWebhookDataDto Data
    );
 
    public record MercadoPagoWebhookDataDto(string Id);
 
    public record PaymentHistoryDto(
        Guid Id,
        decimal Amount,
        DateTime Date,
        string Method,
        string Status,
        string? GatewayTransactionId
    );

}
