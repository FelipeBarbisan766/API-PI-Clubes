using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponseDto?> GetActiveByAdminAsync(Guid adminId);
        Task<bool> CheckAccessAsync(Guid adminId);
        Task RenewAsync(Guid adminId, Guid paymentId);
        Task CancelAsync(Guid subscriptionId);
        Task ExpireOverdueAsync(); // chamado pelo job agendado

    }
}
