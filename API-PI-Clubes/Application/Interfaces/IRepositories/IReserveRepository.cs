using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IReserveRepository
    {
        Task<IEnumerable<Reserve>> GetAllAsync(CancellationToken cancellationToken);
        Task<Reserve?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Reserve>> GetAllByClubIdAsync(Guid clubId, CancellationToken cancellationToken);
        Task<(IEnumerable<Reserve> Items, int TotalCount)> GetAllDetailedByClubIdAsync(Guid clubId,
            ReserveQueryDTO query, CancellationToken cancellationToken);
        Task<(IEnumerable<Reserve> Items, int TotalCount)> GetAllDetailedByPlayerIdAsync(Guid playerId,
            ReserveQueryDTO query, CancellationToken cancellationToken);
        Task<Reserve?> GetByIdWithClubAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Reserve Reserve, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        void Update(Reserve Reserve);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<int> DeleteOldReservesAsync(DateTime cutoffDate);
    }
}
