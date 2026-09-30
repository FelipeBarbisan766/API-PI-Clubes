using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

public class FlagRepository : IFlagRepository
{
    private readonly AppDbContext _context;

    public FlagRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Flag>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Flags
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Flag?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Flags
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<List<FlagPlayer>> GetByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken)
    {
        return await _context.FlagPlayers
            .AsNoTracking()
            .Include(fp => fp.Flag)
            .Where(fp => fp.PlayerId == playerId)
            .OrderByDescending(fp => fp.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForReserveAsync(Guid reserveId, Guid flagId, CancellationToken cancellationToken)
    {
        return await _context.FlagPlayers
            .AnyAsync(fp => fp.ReserveId == reserveId && fp.FlagId == flagId, cancellationToken);
    }

    public async Task AddPlayerFlagAsync(FlagPlayer flagPlayer, CancellationToken cancellationToken)
    {
        await _context.FlagPlayers.AddAsync(flagPlayer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}