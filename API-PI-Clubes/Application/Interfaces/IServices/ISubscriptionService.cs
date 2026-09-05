using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponseDto?> GetActiveByAdminAsync(Guid adminId);
        Task<bool> CheckAccessAsync(Guid adminId);
        Task CancelAsync(Guid subscriptionId, Guid userId);
        Task ExpireOverdueAsync();
    }
}