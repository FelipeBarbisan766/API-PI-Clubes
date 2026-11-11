using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using API_PI_Clubes.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration
{
    public static class RootSeed
    {
        public static readonly Guid RootUserId = new("99999999-0000-0000-0000-000000000001");
        public static readonly DateTime SeedDate = new(2026, 11, 10, 0, 0, 0, DateTimeKind.Utc);
        public const string RootEmail = "fgatechnology76@gmail.com";
    }
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(a => a.Id);

            builder.OwnsOne(u => u.EmailVerification, ev =>
            {
                ev.HasData(new
                {
                    UserId = RootSeed.RootUserId,
                    IsConfirmed = true,
                    VerifiedAt = (DateTime?)RootSeed.SeedDate
                });
            });
            builder.OwnsOne(x => x.ResetPassword);
            
            builder.Property(u => u.CpfEncrypted).HasMaxLength(256);
            builder.Property(u => u.CpfHash).HasMaxLength(64);

            builder.HasIndex(u => u.CpfHash).IsUnique().HasFilter("[CpfHash] IS NOT NULL");
            
            builder.HasIndex(u => u.Email).IsUnique();   

            builder.HasData(new
            {
                Id = RootSeed.RootUserId,
                Name = "Root",
                Email = RootSeed.RootEmail,
                PasswordHash = (string?)null,
                Role = RoleEnum.Root,
                Provider = "google",
                CreatedAt = RootSeed.SeedDate,
                IsActive = true
            });
        }
    }
}
