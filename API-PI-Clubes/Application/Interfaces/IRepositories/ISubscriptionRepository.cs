using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Subscription?> GetActiveByAdminIdAsync(Guid adminId, CancellationToken cancellationToken);
    Task<Subscription?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken);
    Task<IEnumerable<Subscription>> GetExpiredAsync();
    Task<bool> IsOwnedByUserAsync(Guid subscriptionId, Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Subscription subscription,  CancellationToken cancellationToken);
    Task UpdateAsync(Subscription subscription);
    Task<PlanLimitsDTO?> GetActivePlanLimitsByAdminIdAsync(Guid adminId, CancellationToken cancellationToken);
    Task<PlanLimitsDTO?> GetActivePlanLimitsByUserIdAsync(Guid userId,CancellationToken cancellationToken);

}