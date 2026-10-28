using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Services;

public class PlanPricingCalculator : IPlanPricingCalculator
{
    private readonly IPlanPricingRepository _repository;

    public PlanPricingCalculator(IPlanPricingRepository repository) => _repository = repository;

    public async Task<decimal> CalculateAsync(CustomPlanRequestDTO req, CancellationToken cancellationToken)
    {
        var prices = await _repository.GetPricesAsync(cancellationToken);

        var monthly = req.ClubQuantity * GetPrice(prices, PricingComponent.Club)
                      + req.CourtQuantity * GetPrice(prices, PricingComponent.Court);

        return decimal.Round(monthly * req.DurationMonths, 2);
    }

    private static decimal GetPrice(Dictionary<PricingComponent, decimal> prices, PricingComponent component)
        => prices.TryGetValue(component, out var price)
            ? price
            : throw new InvalidOperationException($"Preço não configurado para {component}.");
}