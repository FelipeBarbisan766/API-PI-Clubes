using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;

namespace API_PI_Clubes.Application.Services;

public class ReserveCleanupService : IReserveCleanupService
{
    private readonly IReserveRepository _reserveRepository;
    private readonly ILogger<ReserveCleanupService> _logger;

    private readonly int RetentionMonths;
    
    public ReserveCleanupService(
        IReserveRepository reserveRepository,
        ILogger<ReserveCleanupService> logger,
        IConfiguration configuration)
    {
        _reserveRepository = reserveRepository;
        _logger = logger;
        RetentionMonths = configuration.GetValue<int>("ReserveCleanup:RetentionMonths");
    }

    public async Task<int> CleanupOldReservesAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-RetentionMonths);

        var deletedCount = await _reserveRepository.DeleteOldReservesAsync(cutoffDate);

        _logger.LogInformation(
            "Limpeza de reservas: {Count} registros removidos (jogos anteriores a {Cutoff})",
            deletedCount, cutoffDate);

        return deletedCount;
    }
}