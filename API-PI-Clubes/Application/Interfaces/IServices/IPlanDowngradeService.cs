using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlanDowngradeService
    {
        Task EnforcePlanLimitsAsync(Guid adminId, Plan newPlan, CancellationToken cancellationToken);
    }
}