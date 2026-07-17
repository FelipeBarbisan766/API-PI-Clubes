using API_PI_Clubes.Application.DTOs;
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

        public async Task<IEnumerable<Reserve>> GetAllByClubIdAsync(Guid clubId)
        {
            return await _context.Reserves
                .Where(r => r.IsActive &&
                            r.Schedule.Court.ClubId == clubId)
                .Include(r => r.Schedule)
                .ThenInclude(s => s.Court)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Reserve> Items, int TotalCount)> GetAllDetailedByClubIdAsync(Guid clubId, ReserveQueryDTO query)
        {
            var q = _context.Reserves
                .Where(c => c.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                q = q.Where(c => c.Schedule.Court.Club.Name.Contains(query.Name));

            if (query.Status != null && query.Status > 0)
                q = q.Where(c => c.Status == query.Status);

            var totalCount = await q.CountAsync();

            var items =  await q
                .Where(r => r.IsActive && r.Schedule.Court.ClubId == clubId)
                .Include(r => r.Player)
                    .ThenInclude(p => p.User)
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.Court)
                .OrderByDescending(r => r.Date)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
            return (items, totalCount);
        }

        public async Task<(IEnumerable<Reserve> Items, int TotalCount)> GetAllDetailedByPlayerIdAsync(Guid playerId, ReserveQueryDTO query)
        {
            var q = _context.Reserves
                .Where(c => c.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                q = q.Where(c => c.Player.User.Name.Contains(query.Name));

            if (query.Status != null && query.Status > 0)
                q = q.Where(c => c.Status == query.Status);

            var totalCount = await q.CountAsync();
            
            var items =  await q
                .Where(r => r.IsActive && r.PlayerId == playerId)
                .Include(r => r.Schedule)
                    .ThenInclude(s => s.Court)
                    .ThenInclude(s => s.Club)
                .OrderByDescending(r => r.Date)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
            return (items, totalCount);
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