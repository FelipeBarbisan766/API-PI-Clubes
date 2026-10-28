using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories;

public class PlanPricingRepository : IPlanPricingRepository
{
    private readonly AppDbContext _context;

    public PlanPricingRepository(AppDbContext context) => _context = context;

    public async Task<Dictionary<PricingComponent, decimal>> GetPricesAsync(CancellationToken cancellationToken)
        => await _context.PlanPricings
            .AsNoTracking()
            .ToDictionaryAsync(p => p.Component, p => p.UnitPrice, cancellationToken);
}