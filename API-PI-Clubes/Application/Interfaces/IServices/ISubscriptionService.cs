using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ISubscriptionService
    {
        Task<ResponseSubscriptionDTO> GetActiveByAdmin(Guid id);
        Task<ResponseSubscriptionDTO>  CheckAccess(Guid id);
        Task<ResponseSubscriptionDTO> Renew(Guid id, UpdateSubscriptionDTO dto);
        Task Cancel(Guid id);
        Task ExpireOverdue();
    }
}
