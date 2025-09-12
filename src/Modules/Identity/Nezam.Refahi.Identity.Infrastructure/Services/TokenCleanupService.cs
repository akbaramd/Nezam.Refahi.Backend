// -----------------------------------------------------------------------------
// TokenCleanupService.cs - Background service for token maintenance
// -----------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Background service for periodic token cleanup and maintenance
/// Runs every hour to clean up expired, idle, and old revoked tokens
/// </summary>
public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); // Run every hour

    public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Token cleanup service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformCleanupAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token cleanup");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }

        _logger.LogInformation("Token cleanup service stopped");
    }

    private async Task PerformCleanupAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

        try
        {
            var cleanedCount = await tokenService.CleanupExpiredTokensAsync();
            
            if (cleanedCount > 0)
            {
                _logger.LogInformation("Token cleanup completed: {CleanedCount} tokens removed", cleanedCount);
            }
            else
            {
                _logger.LogDebug("Token cleanup completed: no tokens to clean");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to perform token cleanup");
        }
    }
}
