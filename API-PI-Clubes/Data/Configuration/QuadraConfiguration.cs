using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Data.Configuration
{
    public class QuadraConfiguration : IEntityTypeConfiguration<Quadra>
    {
        public void Configure(EntityTypeBuilder<Quadra> builder)
        {
            builder.HasKey(j => j.Id);

            builder.HasOne(j => j.Clube)
                   .WithMany(c => c.Quadras)
                   .HasForeignKey(j => j.ClubeId);
        }
       
    }
}
