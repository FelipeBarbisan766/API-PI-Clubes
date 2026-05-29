using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
 
        builder.HasKey(s => s.Id);
 
        // builder.Property(s => s.TypeAccess)
        //     .HasConversion<string>()
        //     .HasMaxLength(20)
        //     .IsRequired();
 
        builder.Property(s => s.StartDate)
            .HasDefaultValueSql("GETUTCDATE()");
 
        builder.Property(s => s.ExpiresAt)
            .IsRequired();
 
        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);
 
        builder.HasOne(s => s.Admin)
            .WithMany(a => a.Subscriptions)
            .HasForeignKey(s => s.AdminId)
            .OnDelete(DeleteBehavior.Restrict);
 
        builder.HasOne(s => s.Plan)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
 
        builder.HasOne(s => s.Payment)
            .WithOne(p => p.Subscription)
            .HasForeignKey<Subscription>(s => s.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
