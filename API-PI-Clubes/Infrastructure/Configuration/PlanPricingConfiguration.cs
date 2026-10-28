using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration;

public class PlanPricingConfiguration : IEntityTypeConfiguration<PlanPricing>
{
    private static readonly DateTime SeedDate = new(2026, 10, 28, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<PlanPricing> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Component).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
        b.HasIndex(x => x.Component).IsUnique();

        b.HasData(
            new PlanPricing { Id = Guid.Parse("22222222-0000-0000-0000-000000000001"), Component = PricingComponent.Club,  UnitPrice = 30m, UpdatedAt = SeedDate },
            new PlanPricing { Id = Guid.Parse("22222222-0000-0000-0000-000000000002"), Component = PricingComponent.Court, UnitPrice = 10m, UpdatedAt = SeedDate });
    }
}