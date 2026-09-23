using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories;

public interface IClubReviewRepository
{
    Task<bool> ExistsAsync(Guid clubId, Guid playerId);
    Task AddAsync(ClubReview review);
    Task SaveChangesAsync();
    Task<ResponseClubReviewSummaryDTO> GetSummaryByClubIdAsync(Guid clubId,CancellationToken cancellationToken);
}