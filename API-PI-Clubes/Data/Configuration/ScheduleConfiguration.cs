using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Data.Configuration
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.HasKey(h => h.Id);
            
            builder.HasOne(h => h.Court)
                .WithMany(q => q.Schedules)
                .HasForeignKey(h => h.CourtId);
        }
    }
}
