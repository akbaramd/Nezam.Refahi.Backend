using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Infrastructure.Services;

namespace Nezam.Refahi.Recreation.Infrastructure.HostedServices;

/// <summary>
/// Hosted service for seeding tour data on application startup
/// </summary>
public class TourSeedingHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TourSeedingHostedService> _logger;

    public TourSeedingHostedService(
        IServiceProvider serviceProvider,
        ILogger<TourSeedingHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Tour seeding hosted service started");

        try
        {
            // Wait a bit for the application to fully start
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            using var scope = _serviceProvider.CreateScope();
            var tourSeedingService = scope.ServiceProvider.GetRequiredService<ITourSeedingService>();

            _logger.LogInformation("Starting tour data seeding...");
            await tourSeedingService.SeedToursAsync();
            _logger.LogInformation("Tour data seeding completed successfully");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Tour seeding was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during tour seeding");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tour seeding hosted service is stopping");
        await base.StopAsync(cancellationToken);
    }
}