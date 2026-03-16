using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Data.Configuration
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(h => h.Id);

            builder.HasOne(h => h.User)
                .WithMany(q => q.Players)
                .HasForeignKey(h => h.UserId);
        }
    }
}
