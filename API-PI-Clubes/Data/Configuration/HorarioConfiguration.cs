using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Data.Configuration
{
    public class HorarioConfiguration : IEntityTypeConfiguration<Horario>
    {
        public void Configure(EntityTypeBuilder<Horario> builder)
        {
            builder.HasKey(h => h.Id);
            
            builder.HasOne(h => h.Quadra)
                .WithMany(q => q.Horarios)
                .HasForeignKey(h => h.QuadraId);
        }
    }
}
