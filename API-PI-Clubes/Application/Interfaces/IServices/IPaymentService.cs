using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPaymentService
    {
        Task<PaymentInitiatedDto> InitiateAsync(CreatePaymentDto dto);
        Task HandleWebhookAsync(MercadoPagoWebhookDto webhook);
        Task<IEnumerable<PaymentHistoryDto>> GetHistoryByAdminAsync(Guid adminId);
    }
}
