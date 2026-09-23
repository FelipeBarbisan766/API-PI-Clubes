using API_PI_Clubes.Application.DTOs;
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

        public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
 
        public async Task<Subscription?> GetActiveByAdminIdAsync(Guid adminId, CancellationToken cancellationToken)
            => await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.AdminId == adminId && s.IsActive, cancellationToken);
 
        public async Task<Subscription?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken)
            => await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.PaymentId == paymentId, cancellationToken);
 
        public async Task<IEnumerable<Subscription>> GetExpiredAsync(CancellationToken cancellationToken)
            => await _context.Subscriptions
                .Where(s => s.IsActive && s.ExpiresAt < DateTime.UtcNow)
                .ToListAsync(cancellationToken);
 
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
        public async Task<bool> IsOwnedByUserAsync(Guid subscriptionId, Guid userId, CancellationToken cancellationToken)
            => await _context.Subscriptions
                .AnyAsync(s => s.Id == subscriptionId && s.Admin.UserId == userId, cancellationToken);
    
        public async Task<PlanLimitsDTO?> GetActivePlanLimitsByAdminIdAsync(Guid adminId, CancellationToken cancellationToken)
            => await _context.Subscriptions
                .Where(s => s.AdminId == adminId && s.IsActive)
                .Select(s => new PlanLimitsDTO
                {
                    PlanName = s.Plan.Name,
                    QuantClub = s.Plan.QuantClub,
                    QuantCourt = s.Plan.QuantCourt
                })
                .FirstOrDefaultAsync(cancellationToken);
        
        public async Task<PlanLimitsDTO?> GetActivePlanLimitsByUserIdAsync(Guid userId,CancellationToken cancellationToken)
            => await _context.Subscriptions
                .Where(s => s.Admin.UserId == userId && s.IsActive)
                .Select(s => new PlanLimitsDTO
                {
                    PlanName = s.Plan.Name,
                    QuantClub = s.Plan.QuantClub,
                    QuantCourt = s.Plan.QuantCourt
                })
                .FirstOrDefaultAsync( cancellationToken);
    }
}
