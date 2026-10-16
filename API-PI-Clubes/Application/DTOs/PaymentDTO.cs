using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    // O Angular só envia o plano. AdminId vem do token e o método é escolhido no checkout do Mercado Pago.
    public record CreatePaymentDto(
        Guid PlanId
    );

    public record PaymentInitiatedDto(
        Guid PaymentId,
        string CheckoutUrl
    );

    // Corpo do webhook de Orders:
    // { "action": "order.processed", "type": "order", "data": { "id": "ORD01..." }, ... }
    public record MercadoPagoWebhookDto(
        string? Action,
        string? Type,
        MercadoPagoWebhookDataDto? Data
    );

    public record MercadoPagoWebhookDataDto(string? Id);

    public record PaymentHistoryDto(
        Guid Id,
        decimal Amount,
        DateTime Date,
        string? Method,
        string Status,
        string? MercadoPagoPaymentId
    );
}