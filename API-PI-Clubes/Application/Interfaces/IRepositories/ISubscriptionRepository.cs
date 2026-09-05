using API_PI_Clubes.Model;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id);
    Task<Subscription?> GetActiveByAdminIdAsync(Guid adminId);
    Task<Subscription?> GetByPaymentIdAsync(Guid paymentId);
    Task<IEnumerable<Subscription>> GetExpiredAsync();
    Task<bool> IsOwnedByUserAsync(Guid subscriptionId, Guid userId);
    Task AddAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
}