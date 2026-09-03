using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    private static readonly DateTime SeedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");
 
        builder.HasKey(p => p.Id);
 
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
 
        builder.Property(p => p.Description)
            .HasMaxLength(500);
 
        builder.Property(p => p.Price)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
 
        builder.Property(p => p.QuantClub)
            .IsRequired();
 
        builder.Property(p => p.QuantCourt)
            .IsRequired();
 
        builder.Property(p => p.DurationDays)
            .IsRequired();
 
        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);
 
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasData(
            new Plan
            {
                Id = new Guid("11111111-0000-0000-0000-000000000001"), 
                Name = "Free",
                Description = "Plano Basico Gratuito", 
                Price = 0m, 
                QuantClub = 1, 
                QuantCourt = 0, 
                DurationDays = 30,
                IsActive = true, 
                CreatedAt = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc) // Substitua sua SeedDate por uma data fixa
            } 
        );
    }
}
