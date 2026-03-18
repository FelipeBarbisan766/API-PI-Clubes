using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Data.Configuration
{
    public class ReserveConfiguration : IEntityTypeConfiguration<Reserve>
    {
        public void Configure(EntityTypeBuilder<Reserve> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Schedule)
                   .WithMany(b => b.Reserves)
                   .HasForeignKey(a => a.ScheduleId);

            builder.HasOne(a => a.Player)
                   .WithMany(b => b.Reserves)
                   .HasForeignKey(a => a.PlayerId);
        }
    }
}
