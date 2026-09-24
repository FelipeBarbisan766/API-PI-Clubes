using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPaymentService
    {
        Task<PaymentInitiatedDto> InitiateAsync(CreatePaymentDto dto, Guid adminId, CancellationToken cancellationToken);
        Task HandleWebhookAsync(MercadoPagoWebhookDto webhook, string? signatureHeader, string? requestIdHeader, CancellationToken cancellationToken);
        Task<IEnumerable<PaymentHistoryDto>> GetHistoryByAdminAsync(Guid adminId, CancellationToken cancellationToken);
    }
}