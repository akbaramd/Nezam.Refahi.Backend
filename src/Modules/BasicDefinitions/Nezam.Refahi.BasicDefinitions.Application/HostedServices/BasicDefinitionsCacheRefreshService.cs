using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;

namespace Nezam.Refahi.BasicDefinitions.Application.HostedServices;

/// <summary>
/// Background service that periodically refreshes the BasicDefinitions cache
/// Runs every 3 minutes to keep cache fresh
/// </summary>
public class BasicDefinitionsCacheRefreshService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BasicDefinitionsCacheRefreshService> _logger;
    private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(3);

    public BasicDefinitionsCacheRefreshService(
        IServiceProvider serviceProvider,
        ILogger<BasicDefinitionsCacheRefreshService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BasicDefinitions cache refresh service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RefreshCacheAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while refreshing BasicDefinitions cache");
            }

            try
            {
                await Task.Delay(_refreshInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
        }

        _logger.LogInformation("BasicDefinitions cache refresh service stopped");
    }

    private async Task RefreshCacheAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var cacheService = scope.ServiceProvider.GetRequiredService<IBasicDefinitionsCacheService>();

        try
        {
            _logger.LogDebug("Refreshing BasicDefinitions cache...");
            await cacheService.RefreshCacheAsync();
            
            var stats = await cacheService.GetCacheStatisticsAsync();
            _logger.LogDebug("Cache refresh completed. Capabilities: {CapabilityCount}, Features: {FeatureCount}, Age: {CacheAge}",
                stats.CapabilityCount, stats.FeatureCount, stats.CacheAge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh BasicDefinitions cache");
        }
    }
}
