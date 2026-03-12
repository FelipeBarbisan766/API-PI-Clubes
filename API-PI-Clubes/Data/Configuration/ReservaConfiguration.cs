using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Data.Configuration
{
    public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> builder)
        {
            builder.HasKey(j => j.Id);

            builder.HasOne(j => j.Quadra)
                   .WithMany(c => c.Reservas)
                   .HasForeignKey(j => j.QuadraId);

            builder.HasOne(j => j.Horario)
                   .WithMany(c => c.Reservas)
                   .HasForeignKey(j => j.HorarioId);
        }
    }
}
