using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetByIdAsync(Guid id);
        Task<Subscription?> GetActiveByAdminIdAsync(Guid adminId);
        Task<Subscription?> GetByPaymentIdAsync(Guid paymentId);
        Task<IEnumerable<Subscription>> GetExpiredAsync();
        Task AddAsync(Subscription subscription);
        Task UpdateAsync(Subscription subscription);

    }
}
