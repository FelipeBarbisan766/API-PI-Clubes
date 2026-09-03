using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration
{
    public class PlayerFavoriteSportConfiguration : IEntityTypeConfiguration<PlayerFavoriteSport>
    {
        public void Configure(EntityTypeBuilder<PlayerFavoriteSport> builder)
        {
            builder.HasKey(pf => new { pf.PlayerId, pf.SportId });

            builder.HasOne(pf => pf.Player)
                .WithMany(p => p.FavoriteSports)
                .HasForeignKey(pf => pf.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pf => pf.Sport)
                .WithMany()
                .HasForeignKey(pf => pf.SportId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}