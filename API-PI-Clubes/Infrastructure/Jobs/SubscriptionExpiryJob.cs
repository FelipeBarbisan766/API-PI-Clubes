using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace API_PI_Clubes.Infrastructure.Jobs;

public class SubscriptionExpiryJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubscriptionExpiryJob> _logger;   

    public SubscriptionExpiryJob(    
        IServiceScopeFactory scopeFactory,
        ILogger<SubscriptionExpiryJob> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var subscriptionService = scope.ServiceProvider
                    .GetRequiredService<ISubscriptionService>();
 
                await subscriptionService.ExpireOverdueAsync();
                _logger.LogInformation("SubscriptionExpiryJob executado: {time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao expirar subscriptions.");
            }
 
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
