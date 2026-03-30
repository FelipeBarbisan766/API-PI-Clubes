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
                .ToListAsync();
        }

        public async Task<Club?> GetByIdAsync(Guid id)
        {
            return await _context.Clubs
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Clubs
                .AnyAsync(s => s.Id == id && s.IsActive);
        }

        public async Task AddAsync(Club Club)
        {
            await _context.Clubs.AddAsync(Club);
        }

        public void Update(Club Club)
        {
            _context.Clubs.Update(Club);
        }

        public async Task DeleteAsync(Guid id)
        {
            var Club = await _context.Clubs.FindAsync(id);
            if (Club != null)
            {
                Club.IsActive = false;
                Club.UpdatedAt = DateTime.UtcNow;
                _context.Clubs.Update(Club);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
