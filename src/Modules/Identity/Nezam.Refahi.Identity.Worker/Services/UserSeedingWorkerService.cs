using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Infrastructure.ACL.Contracts;

namespace Nezam.Refahi.Identity.Worker.Services;

/// <summary>
/// Background service for user seeding operations - runs once on startup
/// </summary>
public class UserSeedingWorkerService : BackgroundService
{
    private readonly ILogger<UserSeedingWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public UserSeedingWorkerService(
        ILogger<UserSeedingWorkerService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UserSeedingWorkerService started - running user seeding once on startup");

        try
        {
            await ExecuteUserSeedingAsync(stoppingToken);
            _logger.LogInformation("User seeding completed successfully. Service will now stop.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User seeding failed");
            throw;
        }
    }

    /// <summary>
    /// Executes the user seeding operation once
    /// </summary>
    private async Task ExecuteUserSeedingAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var orchestrator = scope.ServiceProvider.GetRequiredService<IUserSeedOrchestrator>();
        
        try
        {
            _logger.LogInformation("🚀 Starting user seeding operation");

            var result = await ProcessUserSeedingBatchesAsync(orchestrator, cancellationToken);
            
            _logger.LogInformation("🎉 User seeding operation completed successfully! " +
                "Total processed: {TotalProcessed}, Skipped: {Skipped}, Failed: {Failed}",
                result.TotalProcessed, result.Skipped, result.Failed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User seeding operation failed");
            throw;
        }
    }

    /// <summary>
    /// Processes user seeding in batches with simple counter-based approach
    /// </summary>
    private async Task<UserSeedingResult> ProcessUserSeedingBatchesAsync(
        IUserSeedOrchestrator orchestrator, 
        CancellationToken cancellationToken)
    {
        var totalProcessed = 0;
        var totalSkipped = 0;
        var totalFailed = 0;
        var batchNumber = 1;
        var hasMoreUsers = true;

        // Simple configuration
        const int batchSize = 1000;
        const int maxBatches = 1000;
        const int maxParallel = 4;
        const int batchDelaySeconds = 5;

        // Simple watermark: just store the offset
        var watermark = new UserSeedWatermark
        {
            LastProcessedId = "0", // Start from offset 0
            LastProcessedUpdatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("🚀 Starting user seeding with batch size {BatchSize}", batchSize);

        while (hasMoreUsers && batchNumber <= maxBatches && !cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("🔄 Processing batch {BatchNumber} - offset: {Offset}", 
                batchNumber, watermark.LastProcessedId);

            var batchStartTime = DateTime.UtcNow;
            
            try
            {
                var result = await orchestrator.RunIncrementalAsync(
                    watermark: watermark,
                    batchSize: batchSize,
                    maxParallel: maxParallel,
                    dryRun: false,
                    cancellationToken: cancellationToken);

                var batchDuration = DateTime.UtcNow - batchStartTime;

                // Update totals
                totalProcessed += result.TotalProcessed;
                totalSkipped += result.Skipped;
                totalFailed += result.Failed;

                _logger.LogInformation("✅ Batch {BatchNumber} completed in {Duration:F1}s | " +
                    "Processed: {Processed} users | " +
                    "Overall: {TotalProcessed} processed, {TotalSkipped} skipped, {TotalFailed} failed",
                    batchNumber, batchDuration.TotalSeconds, 
                    result.TotalProcessed,
                    totalProcessed, totalSkipped, totalFailed);

                // Check if we have more users to process
                hasMoreUsers = result.TotalProcessed >= batchSize;

                if (hasMoreUsers && result.LastWatermark != null)
                {
                    // Update watermark for next batch
                    watermark = result.LastWatermark;
                    batchNumber++;

                    // Delay between batches
                    if (batchDelaySeconds > 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(batchDelaySeconds), cancellationToken);
                    }
                }
                else
                {
                    // No more users or no watermark - stop
                    hasMoreUsers = false;
                }

                // Log errors from this batch
                if (result.Errors.Any())
                {
                    _logger.LogWarning("Batch {BatchNumber} completed with {ErrorCount} errors", batchNumber, result.Errors.Count);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("User seeding operation cancelled during batch {BatchNumber}", batchNumber);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch {BatchNumber}", batchNumber);
                totalFailed += batchSize;
                batchNumber++;
                
                if (batchNumber > maxBatches)
                {
                    _logger.LogError("Maximum batch limit reached due to errors");
                    break;
                }
            }
        }

        var stopReason = batchNumber > maxBatches ? "reached maximum batch limit" : "no more users to process";
        var successRate = totalProcessed > 0 ? Math.Round((double)(totalProcessed - totalFailed) / totalProcessed * 100, 1) : 0;

        _logger.LogInformation("🎉 User seeding operation completed! " +
            "\n📊 Final Summary:" +
            "\n   • Total batches processed: {BatchCount}" +
            "\n   • Total users processed: {TotalProcessed}" +
            "\n   • Successfully created: {SuccessfullyCreated}" +
            "\n   • Skipped (duplicates): {TotalSkipped}" +
            "\n   • Failed: {TotalFailed}" +
            "\n   • Success rate: {SuccessRate}%" +
            "\n   • Stopped because: {StopReason}",
            batchNumber, totalProcessed, totalProcessed - totalFailed, totalSkipped, totalFailed, 
            successRate, stopReason);

        return new UserSeedingResult
        {
            TotalProcessed = totalProcessed,
            Skipped = totalSkipped,
            Failed = totalFailed,
            BatchesProcessed = batchNumber
        };
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UserSeedingWorkerService is stopping");
        await base.StopAsync(cancellationToken);
    }
}

/// <summary>
/// Result of user seeding operation
/// </summary>
public class UserSeedingResult
{
    public int TotalProcessed { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
    public int BatchesProcessed { get; set; }
}