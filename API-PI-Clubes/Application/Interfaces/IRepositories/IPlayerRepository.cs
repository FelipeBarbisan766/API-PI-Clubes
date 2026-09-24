using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IPlayerRepository
    {
        Task<IEnumerable<Player>> GetAllAsync(CancellationToken cancellationToken);
        Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Player?> GetByUserIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Guid> GetIdByUserIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Player?> GetByIdWithFavoriteSportsAsync(Guid id, CancellationToken cancellationToken);
        Task<Player?> GetByUserIdWithFavoriteSportsAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Player Player, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        void Update(Player Player);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        IExecutionStrategy CreateExecutionStrategy();
        Task<IDbContextTransaction> BeginTransactionAsync( CancellationToken cancellationToken);
        Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId, CancellationToken cancellationToken);
        Task<Player?> GetByProfileNameWithFavoriteSportsAsync(string profileName, CancellationToken cancellationToken);
        Task<bool> ExistsByProfileNameAsync(string profileName, Guid excludeId, CancellationToken cancellationToken);

    }
}
