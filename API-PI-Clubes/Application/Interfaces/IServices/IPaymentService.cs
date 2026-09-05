using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPaymentService
    {
        Task<PaymentInitiatedDto> InitiateAsync(CreatePaymentDto dto, Guid adminId);
        Task HandleWebhookAsync(MercadoPagoWebhookDto webhook, string? signatureHeader, string? requestIdHeader);
        Task<IEnumerable<PaymentHistoryDto>> GetHistoryByAdminAsync(Guid adminId);
    }
}