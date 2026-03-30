using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class ReserveRepository : IReserveRepository
    {
        private readonly AppDbContext _context;

        public ReserveRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reserve>> GetAllAsync()
        {
            return await _context.Reserves
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Reserve?> GetByIdAsync(Guid id)
        {
            return await _context.Reserves
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Reserves
                .AnyAsync(s => s.Id == id && s.IsActive);
        }

        public async Task AddAsync(Reserve Reserve)
        {
            await _context.Reserves.AddAsync(Reserve);
        }

        public void Update(Reserve Reserve)
        {
            _context.Reserves.Update(Reserve);
        }

        public async Task DeleteAsync(Guid id)
        {
            var Reserve = await _context.Reserves.FindAsync(id);
            if (Reserve != null)
            {
                Reserve.IsActive = false;
                Reserve.UpdatedAt = DateTime.UtcNow;
                _context.Reserves.Update(Reserve);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    
    }
}
