using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;

namespace API_PI_Clubes.Application.Services;

public class ReserveCleanupService : IReserveCleanupService
{
    private readonly IReserveRepository _reserveRepository;
    private readonly ILogger<ReserveCleanupService> _logger;
    private const int RetentionMonths = 6;

    public ReserveCleanupService(
        IReserveRepository reserveRepository,
        ILogger<ReserveCleanupService> logger)
    {
        _reserveRepository = reserveRepository;
        _logger = logger;
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