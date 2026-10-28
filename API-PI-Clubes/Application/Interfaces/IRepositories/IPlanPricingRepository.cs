using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Interfaces.IRepositories;

public interface IPlanPricingRepository
{
    Task<Dictionary<PricingComponent, decimal>> GetPricesAsync(CancellationToken cancellationToken);
}