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

        public async Task<IEnumerable<Club>> GetAllAsync()
        {
            return await _context.Clubs
                .Where(c => c.IsActive)
                .Include(c => c.Courts.Where(co => co.IsActive))
                .ToListAsync();
        }

        public async Task<Club?> GetByIdAsync(Guid id)
        {
            return await _context.Clubs
                .Where(u => u.Id == id && u.IsActive)
                .Include(c => c.Courts.Where(co => co.IsActive))
                .AsNoTracking()
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