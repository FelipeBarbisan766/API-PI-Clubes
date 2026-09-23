using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IClubRepository
    {
        Task<(IEnumerable<ResponseClubDTO> Items, int TotalCount)> GetAllAsync(ClubQueryDTO query, CancellationToken cancellationToken);
        Task<Club> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<ResponseClubDTO>> GetAllByAdminIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Club?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken);
        Task<ResponseDashboardDTO?> GetDashboardAsync(Guid clubId, CancellationToken cancellationToken);
        Task AddAsync(Club entity, CancellationToken cancellationToken);
        Task AddClubAdminAsync(ClubAdmin clubAdmin, CancellationToken cancellationToken);
        void Update(Club entity);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<bool> IsOwnedByUserAsync(Guid clubId, Guid userId, CancellationToken cancellationToken);
        Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<ClubCourtUsageDTO>> GetClubsWithCourtCountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
