using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices;

public interface IPlanPricingCalculator
{
    Task<decimal> CalculateAsync(CustomPlanRequestDTO req, CancellationToken cancellationToken);
}
