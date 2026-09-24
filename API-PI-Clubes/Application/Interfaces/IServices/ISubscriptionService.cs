using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponseDto?> GetActiveByAdminAsync(Guid userId ,CancellationToken cancellationToken);
        Task<bool> CheckAccessAsync(Guid userId, CancellationToken cancellationToken);
        Task CancelAsync(Guid subscriptionId, Guid userId, CancellationToken cancellationToken);
        Task ExpireOverdueAsync();
    }
}