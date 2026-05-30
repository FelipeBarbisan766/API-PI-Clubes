using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Subscription?> GetByIdAsync(Guid id)
            => await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == id);
 
        public async Task<Subscription?> GetActiveByAdminIdAsync(Guid adminId)
            => await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.AdminId == adminId && s.IsActive);
 
        public async Task<Subscription?> GetByPaymentIdAsync(Guid paymentId)
            => await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.PaymentId == paymentId);
 
        public async Task<IEnumerable<Subscription>> GetExpiredAsync()
            => await _context.Subscriptions
                .Where(s => s.IsActive && s.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();
 
        public async Task AddAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }
 
        public async Task UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

    
    }
}
