using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlanLimitService
    {
        Task EnsureClubLimitNotReachedAsync(Guid adminId);
        Task EnsureCourtLimitNotReachedAsync(Guid userId, Guid clubId);
        Task<PlanUsageDTO> GetUsageSummaryAsync(Guid userId);
    }
}