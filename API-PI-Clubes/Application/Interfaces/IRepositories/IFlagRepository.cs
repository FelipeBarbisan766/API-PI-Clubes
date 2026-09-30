using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories;

public interface IFlagRepository
{
    Task<List<Flag>> GetAllAsync(CancellationToken cancellationToken);
    Task<Flag?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<FlagPlayer>> GetByPlayerIdAsync(Guid playerId, CancellationToken cancellationToken);
    Task<bool> ExistsForReserveAsync(Guid reserveId, Guid flagId, CancellationToken cancellationToken);
    Task AddPlayerFlagAsync(FlagPlayer flagPlayer, CancellationToken cancellationToken);
}