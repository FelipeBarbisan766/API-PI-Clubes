using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Data.Configuration
{
    public class CourtConfiguration : IEntityTypeConfiguration<Court>
    {
        public void Configure(EntityTypeBuilder<Court> builder)
        {
            builder.HasKey(j => j.Id);

            builder.HasOne(j => j.Club)
                   .WithMany(c => c.Courts)
                   .HasForeignKey(j => j.ClubId);
        }
       
    }
}
