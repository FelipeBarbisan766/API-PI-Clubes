using API_PI_Clubes.Application.Interfaces.IServices;

namespace API_PI_Clubes.Application.Services;

public class ReserveCleanupHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReserveCleanupHostedService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromDays(1); 

    public ReserveCleanupHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<ReserveCleanupHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_interval);

        await RunCleanupAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunCleanupAsync(stoppingToken);
        }
    }

    private async Task RunCleanupAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var cleanupService = scope.ServiceProvider.GetRequiredService<IReserveCleanupService>();
            await cleanupService.CleanupOldReservesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar limpeza de reservas antigas.");
        }
    }
}