using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Club)
                .WithMany(b => b.Images)
                .HasForeignKey(a => a.ClubId);

            builder.HasOne(a => a.Court)
                .WithMany(b => b.Images)
                .HasForeignKey(a => a.CourtId);
        }
    }
}
