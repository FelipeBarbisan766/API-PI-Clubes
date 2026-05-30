using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
            => await _context.Payments.FindAsync(id);
 
        public async Task<Payment?> GetByGatewayTransactionIdAsync(string gatewayTransactionId)
            => await _context.Payments
                .FirstOrDefaultAsync(p => p.GatewayTransactionId == gatewayTransactionId);
 
        public async Task<IEnumerable<Payment>> GetByAdminIdAsync(Guid adminId)
            => await _context.Subscriptions
                .Where(s => s.AdminId == adminId)
                .Include(s => s.Payment)
                .Select(s => s.Payment)
                .ToListAsync();
 
        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }
 
        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

    
    }
}
