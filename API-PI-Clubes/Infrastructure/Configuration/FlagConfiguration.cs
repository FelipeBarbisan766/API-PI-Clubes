using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configuration;

public class FlagConfiguration : IEntityTypeConfiguration<Flag>
{
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Flag> builder)
    {
        builder.Property(f => f.Name).IsRequired().HasMaxLength(100);
        builder.Property(f => f.Description).HasMaxLength(300);
        builder.HasIndex(f => f.Name).IsUnique();

        builder.HasData(
            new Flag { Id = new Guid("22222222-0000-0000-0000-000000000001"), Name = "No-show", Description = "Reservou a quadra e não compareceu, sem avisar.", IsActive = true, CreatedAt = SeedDate },
            new Flag { Id = new Guid("22222222-0000-0000-0000-000000000002"), Name = "Atraso excessivo", Description = "Chegou muito depois do horário reservado, prejudicando o uso da quadra.", IsActive = true, CreatedAt = SeedDate },
            new Flag { Id = new Guid("22222222-0000-0000-0000-000000000003"), Name = "Cancelamento em cima da hora", Description = "Cancelou a reserva sem antecedência, deixando o horário vazio.", IsActive = true, CreatedAt = SeedDate },
            new Flag { Id = new Guid("22222222-0000-0000-0000-000000000004"), Name = "Danos à quadra", Description = "Causou danos à quadra ou aos equipamentos do clube.", IsActive = true, CreatedAt = SeedDate },
            new Flag { Id = new Guid("22222222-0000-0000-0000-000000000005"), Name = "Comportamento inadequado", Description = "Conduta agressiva ou desrespeitosa com outros jogadores ou funcionários.", IsActive = true, CreatedAt = SeedDate },
            new Flag { Id = new Guid("22222222-0000-0000-0000-000000000006"), Name = "Falta de pagamento", Description = "Utilizou a quadra e não realizou o pagamento.", IsActive = true, CreatedAt = SeedDate }
        );
    }
}