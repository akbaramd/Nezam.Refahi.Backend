using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Infrastructure.ACL.Contracts;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Hosted service for automatic user seeding operations
/// Runs periodically to sync users from external sources
/// </summary>
public class UserSeedingHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserSeedingHostedService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(6); // Run every 6 hours

    public UserSeedingHostedService(
        IServiceProvider serviceProvider,
        ILogger<UserSeedingHostedService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("User Seeding Hosted Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformSeedingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user seeding operation");
            }

            await Task.Delay(_period, stoppingToken);
        }

        _logger.LogInformation("User Seeding Hosted Service stopped");
    }

    private async Task PerformSeedingAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var orchestrator = scope.ServiceProvider.GetRequiredService<IUserSeedOrchestrator>();

        _logger.LogInformation("Starting scheduled user seeding operation");

        try
        {
            // First validate configuration
            var validationResult = await orchestrator.ValidateConfigurationAsync(cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Seeding configuration validation failed: {Errors}", 
                    string.Join(", ", validationResult.Errors));
                return;
            }

            if (validationResult.Warnings.Count > 0)
            {
                _logger.LogWarning("Seeding configuration warnings: {Warnings}", 
                    string.Join(", ", validationResult.Warnings));
            }

            // Run incremental seeding (this would use stored watermark)
            var result = await orchestrator.RunIncrementalAsync(
                watermark: new UserSeedWatermark(), // In real implementation, load from storage
                batchSize: 1000,
                maxParallel: 4,
                dryRun: false,
                cancellationToken: cancellationToken);

            _logger.LogInformation("User seeding operation completed: " +
                "Processed: {TotalProcessed}, Created: {SuccessfullyCreated}, " +
                "Skipped: {Skipped}, Failed: {Failed}, Duration: {Duration}",
                result.TotalProcessed, result.SuccessfullyCreated, 
                result.Skipped, result.Failed, result.Duration);

            if (result.Errors.Count > 0)
            {
                _logger.LogWarning("Seeding operation had {ErrorCount} errors", result.Errors.Count);
                foreach (var error in result.Errors.Take(5)) // Log first 5 errors
                {
                    _logger.LogWarning("Seeding error: {ErrorCode} - {Message}", 
                        error.ErrorCode, error.Message);
                }
            }

            if (result.Warnings.Count > 0)
            {
                _logger.LogInformation("Seeding operation had {WarningCount} warnings", result.Warnings.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error during user seeding operation");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("User Seeding Hosted Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
