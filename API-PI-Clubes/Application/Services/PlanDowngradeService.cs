using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Services
{
    using API_PI_Clubes.Application.Interfaces.IServices;

    public class PlanDowngradeService : IPlanDowngradeService
    {
        private readonly IClubRepository _clubRepository;

        public PlanDowngradeService(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }

        public async Task EnforcePlanLimitsAsync(Guid adminId, Plan newPlan, CancellationToken cancellationToken)
        {
            var clubs = await _clubRepository.GetActiveWithActiveCourtsByAdminIdAsync(adminId, cancellationToken);
            if (clubs.Count == 0)
                return;

            var ordered = clubs.OrderBy(c => c.CreatedAt).ToList();
            var changed = false;

            foreach (var club in ordered.Skip(newPlan.QuantClub))
            {
                club.IsClubActive = false;
                foreach (var court in club.Courts)
                    court.IsCourtActive = false;

                changed = true;
            }

            foreach (var club in ordered.Take(newPlan.QuantClub))
            {
                foreach (var court in club.Courts.OrderBy(c => c.CreatedAt).Skip(newPlan.QuantCourt))
                {
                    court.IsCourtActive = false;
                    changed = true;
                }
            }

            if (changed)
                await _clubRepository.SaveChangesAsync(cancellationToken);
        }
    }
}