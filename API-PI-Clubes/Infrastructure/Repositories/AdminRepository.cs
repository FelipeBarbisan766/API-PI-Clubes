using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _context.Admins
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Admin?> GetByIdAsync(Guid id)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Admins
                .AnyAsync(s => s.Id == id && s.IsActive);
        }

        public async Task AddAsync(Admin Admin)
        {
            await _context.Admins.AddAsync(Admin);
        }

        public void Update(Admin Admin)
        {
            _context.Admins.Update(Admin);
        }

        public async Task DeleteAsync(Guid id)
        {
            var Admin = await _context.Admins.FindAsync(id);
            if (Admin != null)
            {
                Admin.IsActive = false;
                Admin.UpdatedAt = DateTime.UtcNow;
                _context.Admins.Update(Admin);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IDisposable> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
