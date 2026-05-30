using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
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
    }
}
