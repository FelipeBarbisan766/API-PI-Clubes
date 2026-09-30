using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration;

public class FlagPlayerConfiguration : IEntityTypeConfiguration<FlagPlayer>
{
    public void Configure(EntityTypeBuilder<FlagPlayer> builder)
    {
        builder.HasIndex(f => new { f.ReserveId, f.FlagId })
            .IsUnique()
            .HasFilter("[ReserveId] IS NOT NULL");   
        builder.HasIndex(f => f.PlayerId);
        builder.HasOne(f => f.Flag).WithMany().HasForeignKey(f => f.FlagId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}