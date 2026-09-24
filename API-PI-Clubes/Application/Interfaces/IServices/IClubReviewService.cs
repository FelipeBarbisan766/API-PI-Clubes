using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices;

public interface IClubReviewService
{
    Task<ResponseClubReviewSummaryDTO> RateClub(Guid userId, Guid clubId, CreateClubReviewDTO dto, CancellationToken cancellationToken);
    Task<ResponseClubReviewSummaryDTO> GetSummary(Guid clubId, CancellationToken cancellationToken);
    Task<Boolean> VerifyReview(Guid userId, Guid clubId, CancellationToken cancellationToken);
}