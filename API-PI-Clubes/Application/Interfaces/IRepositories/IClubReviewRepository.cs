using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories;

public interface IClubReviewRepository
{
    Task<bool> ExistsAsync(Guid clubId, Guid playerId, CancellationToken cancellationToken);
    Task AddAsync(ClubReview review, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<ResponseClubReviewSummaryDTO> GetSummaryByClubIdAsync(Guid clubId,CancellationToken cancellationToken);
}