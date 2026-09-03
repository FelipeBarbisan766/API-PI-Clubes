using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration
{
    public class SportConfiguration : IEntityTypeConfiguration<Sport>
    {
        private static readonly DateTime SeedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Sport> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasData(
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000001"), Name = "Futsal", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000002"), Name = "Basquetebol", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000003"), Name = "Voleibol", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000004"), Name = "Vôlei Sentado", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000005"), Name = "Handebol", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000006"), Name = "Netball", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000007"), Name = "Tênis", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000008"), Name = "Badminton", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000009"), Name = "Squash", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-00000000000a"), Name = "Padel", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-00000000000b"), Name = "Pickleball", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-00000000000c"), Name = "Tênis de Mesa", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-00000000000d"), Name = "Judô", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-00000000000e"), Name = "Karatê", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-00000000000f"), Name = "Taekwondo", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000010"), Name = "Esgrima", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000011"), Name = "Sepak Takraw", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000012"), Name = "Hóquei", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000013"), Name = "Dodgeball", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000014"), Name = "Raquetebol", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000015"), Name = "Pelota Basca", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000016"), Name = "Floorball", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000017"), Name = "Korfball", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000018"), Name = "Tchoukball", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-000000000019"), Name = "Goalball", IsActive = true, CreatedAt = SeedDate },
                new Sport { Id = new Guid("11111111-0000-0000-0000-00000000001a"), Name = "Futebol", IsActive = true, CreatedAt = SeedDate }
            );
        }
    }
}