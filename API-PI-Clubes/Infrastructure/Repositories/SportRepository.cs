using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class SportRepository : ISportRepository
    {
        private readonly AppDbContext _context;

        public SportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountExistingAsync(List<Guid> ids,CancellationToken cancellationToken)
        {
            return await _context.Sports
                .Where(s => s.IsActive && ids.Contains(s.Id))
                .CountAsync(cancellationToken);
        }

        public async Task<List<ResponseSportDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Sports
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new ResponseSportDTO { Id = s.Id, Name = s.Name })
                .ToListAsync(cancellationToken);
        }
        public async Task<List<ResponseSportDTO>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            return await _context.Sports
                .Where(s => ids.Contains(s.Id) && s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new ResponseSportDTO { Id = s.Id, Name = s.Name })
                .ToListAsync(cancellationToken);
        }
        public async Task AddAsync(Sport sport, CancellationToken cancellationToken)
        {
            await _context.Sports.AddAsync(sport, cancellationToken);
        }
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Sports
                .AnyAsync(s => s.Id == id && s.IsActive, cancellationToken);
        }
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            await _context.Sports
                .Where(s => s.IsActive && s.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.IsActive, false)
                    .SetProperty(x => x.UpdatedAt, now), cancellationToken);
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}