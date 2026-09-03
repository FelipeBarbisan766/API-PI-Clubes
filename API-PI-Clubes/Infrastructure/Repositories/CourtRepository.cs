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

        public async Task<(IEnumerable<ResponseCourtDTO> Items, int TotalCount)> GetAllAsync(CourtQueryDTO query)
        {
            var q = _context.Courts
                .Where(c => c.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                q = q.Where(c => c.Name.Contains(query.Name));

            if (!string.IsNullOrWhiteSpace(query.City))
                q = q.Where(c => c.Club.Address.City.Contains(query.City));

            if (query.SportIds != null && query.SportIds.Count > 0)
                q = q.Where(c => c.CourtSports.Any(cs => query.SportIds.Contains(cs.SportId)));

            var totalCount = await q.CountAsync();

            var items = await q
                .Select(c => new ResponseCourtDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Surface = c.Surface,
                    IsCovered = c.IsCovered,
                    PricePerHour = c.PricePerHour,
                    Description = c.Description,
                    ClubId = c.ClubId,
                    Sports = c.CourtSports
                        .Select(cs => new SportDTO { Id = cs.Sport.Id, Name = cs.Sport.Name })
                        .ToList(),
                    Images = c.Images
                        .OrderBy(i => i.Order)
                        .Select(i => new ImageDTO
                        {
                            Id = i.Id,
                            ThumbUrl = i.ThumbUrl,
                            MediumUrl = i.MediumUrl,
                            FullUrl = i.FullUrl,
                            Order = i.Order
                        })
                        .ToList()
                })
                .OrderBy(c => c.Name)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Court?> GetByIdAsync(Guid id)
        {
            return await _context.Courts
                .Where(u => u.Id == id && u.IsActive)
                .Include(c => c.Images.OrderBy(i => i.Order))
                .Include(c => c.CourtSports)
                .ThenInclude(cs => cs.Sport)
                .FirstOrDefaultAsync();
        }


        public async Task<List<ResponseCourtDTO>> GetAllByClubIdAsync(Guid id)
        {
            return await _context.Courts
                .Where(c => c.ClubId == id && c.IsActive)
                .Select(c => new ResponseCourtDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Surface = c.Surface,
                    IsCovered = c.IsCovered,
                    PricePerHour = c.PricePerHour,
                    Description = c.Description,
                    ClubId = c.ClubId,
                    Sports = c.CourtSports
                        .Select(cs => new SportDTO { Id = cs.Sport.Id, Name = cs.Sport.Name })
                        .ToList(),
                    Images = c.Images
                        .OrderBy(i => i.Order)
                        .Select(i => new ImageDTO
                        {
                            Id = i.Id,
                            ThumbUrl = i.ThumbUrl,
                            MediumUrl = i.MediumUrl,
                            FullUrl = i.FullUrl,
                            Order = i.Order
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<Court?> GetByIdWithImagesAsync(Guid id)
        {
            return await _context.Courts
                .Where(u => u.Id == id && u.IsActive)
                .Include(c => c.Images.OrderBy(i => i.Order))
                .FirstOrDefaultAsync();
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

        public async Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId)
        {
            return await _context.Courts
                .AnyAsync(c => c.Id == Id && c.Club.ClubAdmin.Any(a => a.Admin.UserId == userId));
        }
    }
}