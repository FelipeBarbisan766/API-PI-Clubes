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

        public async Task<int> CountExistingAsync(List<Guid> ids)
        {
            return await _context.Sports
                .Where(s => s.IsActive && ids.Contains(s.Id))
                .CountAsync();
        }

        public async Task<List<ResponseSportDTO>> GetAllAsync()
        {
            return await _context.Sports
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new ResponseSportDTO { Id = s.Id, Name = s.Name })
                .ToListAsync();
        }
        public async Task<List<ResponseSportDTO>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.Sports
                .Where(s => ids.Contains(s.Id) && s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new ResponseSportDTO { Id = s.Id, Name = s.Name })
                .ToListAsync();
        }
    }
}