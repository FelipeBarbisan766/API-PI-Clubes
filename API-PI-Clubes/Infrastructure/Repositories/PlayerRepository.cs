using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _context;

        public PlayerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(Guid id)
        {
            return await _context.Players
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Players
                .AnyAsync(s => s.Id == id && s.IsActive);
        }

        public async Task AddAsync(Player Player)
        {
            await _context.Players.AddAsync(Player);
        }

        public void Update(Player Player)
        {
            _context.Players.Update(Player);
        }

        public async Task DeleteAsync(Guid id)
        {
            var Player = await _context.Players.FindAsync(id);
            if (Player != null)
            {
                Player.IsActive = false;
                Player.UpdatedAt = DateTime.UtcNow;
                _context.Players.Update(Player);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
