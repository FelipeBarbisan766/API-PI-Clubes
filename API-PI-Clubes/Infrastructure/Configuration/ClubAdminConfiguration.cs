using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration
{
    public class ClubAdminConfiguration : IEntityTypeConfiguration<ClubAdmin>
    {
        public void Configure(EntityTypeBuilder<ClubAdmin> builder)
        {
            builder.HasKey(a => new { a.ClubId, a.AdminId });

            builder.HasOne(a => a.Club)
                .WithMany(b => b.ClubAdmin)
                .HasForeignKey(a => a.ClubId);

            builder.HasOne(a => a.Admin)
                .WithMany(b => b.ClubAdmin)
                .HasForeignKey(a => a.AdminId);
        }
    }
}
