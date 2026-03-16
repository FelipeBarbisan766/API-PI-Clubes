using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Data.Configuration
{
    public class ReserveConfiguration : IEntityTypeConfiguration<Reserve>
    {
        public void Configure(EntityTypeBuilder<Reserve> builder)
        {
            builder.HasKey(j => j.Id);

            builder.HasOne(j => j.Schedule)
                   .WithMany(c => c.Reserves)
                   .HasForeignKey(j => j.ScheduleId);

            builder.HasOne(j => j.Player)
                   .WithMany(c => c.Reserves)
                   .HasForeignKey(j => j.PlayerId);
        }
    }
}
