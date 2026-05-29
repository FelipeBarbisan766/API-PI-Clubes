using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPaymentService
    {
        Task<ResponseIdDTO> Create(CreatPaymentDTO dto);
        Task<ResponsePaymentDTO> ConfirmFromWebhook(Guid gatewayId);
    }
}
