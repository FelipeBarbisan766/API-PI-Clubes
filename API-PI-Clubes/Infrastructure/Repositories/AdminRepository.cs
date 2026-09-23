using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByUserIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive, cancellationToken);
        }

        public async Task<Admin?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Admins
                .AnyAsync(s => s.Id == id && s.IsActive, cancellationToken);
        }

        public async Task AddAsync(Admin Admin, CancellationToken cancellationToken)
        {
            await _context.Admins.AddAsync(Admin, cancellationToken);
        }

        public void Update(Admin Admin)
        {
            _context.Admins.Update(Admin);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var Admin = await _context.Admins.FindAsync(new object[] { id }, cancellationToken);
            if (Admin != null)
            {
                Admin.IsActive = false;
                Admin.UpdatedAt = DateTime.UtcNow;
                _context.Admins.Update(Admin);
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }
        public async Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Admins
                .AnyAsync(c => c.Id == Id && c.User.Id == userId, cancellationToken);
        }
    }
}