using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class ClubRepository : IClubRepository
    {
        private readonly AppDbContext _context;

        public ClubRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<ResponseClubDTO> Items, int TotalCount)> GetAllAsync(ClubQueryDTO query)
        {
            var q = _context.Clubs
                .Where(c => c.IsActive)
                .AsQueryable();

            // Filtros
            if (!string.IsNullOrWhiteSpace(query.Name))
                q = q.Where(c => c.Name.Contains(query.Name));

            if (!string.IsNullOrWhiteSpace(query.City))
                q = q.Where(c => c.Address.City.Contains(query.City));

            if (query.Types != null && query.Types.Count > 0)
                q = q.Where(c => c.Courts
                    .Any(co => co.IsActive && query.Types.Contains(co.Type)));

            var totalCount = await q.CountAsync();

            var items = await q
                .Select(c => new ResponseClubDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    Description = c.Description,
                    Street = c.Address.Street,
                    City = c.Address.City,
                    State = c.Address.State,
                    Country = c.Address.Country,
                    MinPrice = c.Courts.Where(co => co.IsActive).Any()
                        ? c.Courts.Where(co => co.IsActive).Min(co => co.PricePerHour)
                        : 0,
                    CourtCount = c.Courts.Count(co => co.IsActive),
                    Types = c.Courts.Where(co => co.IsActive)
                        .Select(co => co.Type).Distinct().ToList(),
                    Images = c.Images
                        .Select(i => new ImageDTO
                        {
                            ThumbUrl  = i.ThumbUrl,
                            MediumUrl = i.MediumUrl,
                            FullUrl   = i.FullUrl
                        })
                        .ToList()
                })
                .OrderBy(c => c.Name)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Club?> GetByIdAsync(Guid id)
        {
            return await _context.Clubs
                .Where(u => u.Id == id && u.IsActive)
                .AsNoTracking()
                .Include(c => c.Images)
                .Include(c => c.Courts.Where(co => co.IsActive))
                    .ThenInclude(co => co.Images)
                .FirstOrDefaultAsync();
        }
        public async Task<List<ResponseClubDTO>> GetAllByAdminIdAsync(Guid id)
        {
            return await _context.Clubs
                .AsQueryable()
                .Where(c => c.ClubAdmin.Any(ca => ca.AdminId == id) && c.IsActive)
                .Select(c => new ResponseClubDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    Description = c.Description,
                    Street = c.Address.Street,
                    City = c.Address.City,
                    State = c.Address.State,
                    Country = c.Address.Country,

                    MinPrice = c.Courts.Where(co => co.IsActive).Any()
                        ? c.Courts.Where(co => co.IsActive).Min(co => co.PricePerHour)
                        : 0,

                    CourtCount = c.Courts.Count(co => co.IsActive),

                    Types = c.Courts
                        .Where(co => co.IsActive)
                        .Select(co => co.Type)
                        .Distinct()
                        .ToList(),

                    Images      = c.Images
                        .Select(i => new ImageDTO
                        {
                            ThumbUrl  = i.ThumbUrl,
                            MediumUrl = i.MediumUrl,
                            FullUrl   = i.FullUrl
                        })
                        .ToList()

                })
                .ToListAsync();
        }
        public async Task<Club?> GetByIdWithImagesAsync(Guid id)
        {
            return await _context.Clubs
                .Where(u => u.Id == id && u.IsActive)
                .Include(c => c.Images)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Clubs
                .AnyAsync(s => s.Id == id && s.IsActive);
        }

        public async Task AddAsync(Club club)
        {
            await _context.Clubs.AddAsync(club);
        }

        public async Task AddClubAdminAsync(ClubAdmin clubAdmin)
        {
            await _context.ClubAdmins.AddAsync(clubAdmin);
        }

        public void Update(Club club)
        {
            _context.Clubs.Update(club);
        }

        public async Task DeleteAsync(Guid id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club != null)
            {
                club.IsActive = false;
                club.UpdatedAt = DateTime.UtcNow;
                _context.Clubs.Update(club);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}