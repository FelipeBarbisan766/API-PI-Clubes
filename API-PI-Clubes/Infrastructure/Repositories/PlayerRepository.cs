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

        public async Task<IEnumerable<Player>> GetAllAsync( CancellationToken cancellationToken)
        {
            return await _context.Players
                .Where(c => c.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Players
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive,cancellationToken);
        }

        public async Task<Player?> GetByUserIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Players
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive,cancellationToken);
        }
        public async Task<Guid> GetIdByUserIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Players
                .Where(u => u.UserId == id && u.IsActive)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Player?> GetByIdWithFavoriteSportsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Players
                .Where(u => u.Id == id && u.IsActive)
                .Include(p => p.FavoriteSports)
                .ThenInclude(fs => fs.Sport)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Player?> GetByUserIdWithFavoriteSportsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Players
                .Where(u => u.UserId == userId && u.IsActive)
                .Include(p => p.FavoriteSports)
                .ThenInclude(fs => fs.Sport)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Players
                .AnyAsync(s => s.Id == id && s.IsActive,cancellationToken);
        }

        public async Task AddAsync(Player Player, CancellationToken cancellationToken)
        {
            await _context.Players.AddAsync(Player,cancellationToken);
        }

        public void Update(Player Player)
        {
            _context.Players.Update(Player);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var Player = await _context.Players.FindAsync(new object[] { id },cancellationToken);
            if (Player != null)
            {
                Player.IsActive = false;
                Player.UpdatedAt = DateTime.UtcNow;
                _context.Players.Update(Player);
            }
        }

        public async Task SaveChangesAsync( CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync( CancellationToken cancellationToken)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Players
                .AnyAsync(c => c.Id == Id && c.User.Id == userId,cancellationToken);
        }

        public async Task<Player?> GetByProfileNameWithFavoriteSportsAsync(string profileName, CancellationToken cancellationToken)
        {
            return await _context.Players
                .Where(p => p.ProfileName == profileName && p.IsActive)
                .Include(p => p.FavoriteSports)
                .ThenInclude(fs => fs.Sport)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsByProfileNameAsync(string profileName, Guid excludeId, CancellationToken cancellationToken)
        {
            return await _context.Players
                .AnyAsync(p => p.ProfileName == profileName && p.Id != excludeId && p.IsActive,cancellationToken);
        }
    }
}