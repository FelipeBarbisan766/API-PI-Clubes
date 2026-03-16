using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Data.Configuration
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.HasKey(h => h.Id);

        builder.HasOne(h => h.Clube)
            .WithMany(q => q.Admins)
            .HasForeignKey(h => h.ClubeId);

        builder.HasOne(h => h.User)
            .WithMany(q => q.Admins)
            .HasForeignKey(h => h.UserId);


        }
}
