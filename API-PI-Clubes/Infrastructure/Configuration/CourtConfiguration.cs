using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration
{
    public class CourtConfiguration : IEntityTypeConfiguration<Court>
    {
        public void Configure(EntityTypeBuilder<Court> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(c => c.PricePerHour)
            .HasPrecision(10, 2);

            builder.HasOne(a => a.Club)
                   .WithMany(b => b.Courts)
                   .HasForeignKey(a => a.ClubId);
        }
       
    }
}
