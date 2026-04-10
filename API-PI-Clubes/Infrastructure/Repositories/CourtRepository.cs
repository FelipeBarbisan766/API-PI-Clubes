using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class CourtRepository : ICourtRepository
    {
        private readonly AppDbContext _context;

        public CourtRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseCourtDTO>> GetAllAsync()
        {
            return await _context.Courts
                .Where(c => c.IsActive)
                .Include(c => c.Images)
                .Select(c => new ResponseCourtDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Surface = c.Surface,
                    IsCovered = c.IsCovered,
                    PricePerHour = c.PricePerHour,
                    Description = c.Description,
                    ClubId = c.ClubId,
                    ImagesUrls = c.Images.Select(i => i.Url).ToList()
                })
                .ToListAsync();
        }

        public async Task<Court?> GetByIdAsync(Guid id)
        {
            return await _context.Courts
                .Include(c => c.Images)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Courts
                .AnyAsync(s => s.Id == id && s.IsActive);
        }

        public async Task AddAsync(Court Court)
        {
            await _context.Courts.AddAsync(Court);
        }

        public void Update(Court Court)
        {
            _context.Courts.Update(Court);
        }

        public async Task DeleteAsync(Guid id)
        {
            var Court = await _context.Courts.FindAsync(id);
            if (Court != null)
            {
                Court.IsActive = false;
                Court.UpdatedAt = DateTime.UtcNow;
                _context.Courts.Update(Court);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
