using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration
{
    public class CourtSportConfiguration : IEntityTypeConfiguration<CourtSport>
    {
        public void Configure(EntityTypeBuilder<CourtSport> builder)
        {
            builder.HasKey(cs => new { cs.CourtId, cs.SportId });

            builder.HasOne(cs => cs.Court)
                .WithMany(c => c.CourtSports)
                .HasForeignKey(cs => cs.CourtId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cs => cs.Sport)
                .WithMany(s => s.CourtSports)
                .HasForeignKey(cs => cs.SportId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}