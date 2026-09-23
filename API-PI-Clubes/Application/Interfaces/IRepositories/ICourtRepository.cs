using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface ICourtRepository
    {
        Task<(IEnumerable<ResponseCourtDTO> Items, int TotalCount)> GetAllAsync(CourtQueryDTO query, CancellationToken cancellationToken);
        Task<Court?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<ResponseCourtDTO>> GetAllByClubIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Court?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Court Court, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        void Update(Court Court);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId, CancellationToken cancellationToken);
        Task<int> CountByClubIdAsync(Guid clubId, CancellationToken cancellationToken);
    }
}
