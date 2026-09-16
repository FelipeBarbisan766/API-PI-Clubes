using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices;

public interface IClubReviewService
{
    Task<ResponseClubReviewSummaryDTO> RateClub(Guid userId, Guid clubId, CreateClubReviewDTO dto);
    Task<ResponseClubReviewSummaryDTO> GetSummary(Guid clubId);
}