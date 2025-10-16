using Microsoft.Extensions.Logging;
using Hangfire;
using Nezam.Refahi.Shared.Domain.Repositories;

namespace Nezam.Refahi.Shared.Infrastructure.Outbox;

/// <summary>
/// Service for cleaning up processed outbox messages
/// </summary>
public class OutboxCleanupService
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<OutboxCleanupService> _logger;

    public OutboxCleanupService(
        IOutboxRepository outboxRepository,
        ILogger<OutboxCleanupService> logger)
    {
        _outboxRepository = outboxRepository ?? throw new ArgumentNullException(nameof(outboxRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Cleans up processed outbox messages older than the specified retention period
    /// </summary>
    /// <param name="retentionDays">Number of days to retain processed messages (default: 7 days)</param>
    /// <param name="batchSize">Number of messages to delete in each batch (default: 1000)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of messages cleaned up</returns>
    [AutomaticRetry(Attempts = 3)]
    public async Task<int> CleanupProcessedMessagesAsync(
        int retentionDays = 7, 
        int batchSize = 1000, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting outbox cleanup process - retention: {RetentionDays} days, batch size: {BatchSize}", 
                retentionDays, batchSize);

            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
            var totalCleaned = 0;

            while (true)
            {
                // Get processed messages older than cutoff date
                var messagesToDelete = await _outboxRepository.GetProcessedMessagesOlderThanAsync(
                    cutoffDate, batchSize, cancellationToken);

                if (!messagesToDelete.Any())
                {
                    _logger.LogDebug("No more processed messages older than {CutoffDate} found", cutoffDate);
                    break;
                }

                // Delete the batch
                var deletedCount = await _outboxRepository.DeleteMessagesAsync(
                    messagesToDelete.Select(m => m.Id), cancellationToken);

                totalCleaned += deletedCount;
                _logger.LogDebug("Cleaned up {DeletedCount} processed messages (total: {TotalCleaned})", 
                    deletedCount, totalCleaned);

                // If we got fewer messages than batch size, we're done
                if (messagesToDelete.Count() < batchSize)
                {
                    break;
                }

                // Small delay to prevent overwhelming the database
                await Task.Delay(100, cancellationToken);
            }

            _logger.LogInformation("Outbox cleanup completed - cleaned up {TotalCleaned} processed messages", totalCleaned);
            return totalCleaned;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during outbox cleanup process");
            throw;
        }
    }

    /// <summary>
    /// Cleans up failed messages older than the specified retention period
    /// </summary>
    /// <param name="retentionDays">Number of days to retain failed messages (default: 30 days)</param>
    /// <param name="batchSize">Number of messages to delete in each batch (default: 1000)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of failed messages cleaned up</returns>
    [AutomaticRetry(Attempts = 3)]
    public async Task<int> CleanupFailedMessagesAsync(
        int retentionDays = 30, 
        int batchSize = 1000, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting failed outbox messages cleanup - retention: {RetentionDays} days, batch size: {BatchSize}", 
                retentionDays, batchSize);

            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
            var totalCleaned = 0;

            while (true)
            {
                // Get failed messages older than cutoff date
                var messagesToDelete = await _outboxRepository.GetFailedMessagesOlderThanAsync(
                    cutoffDate, batchSize, cancellationToken);

                if (!messagesToDelete.Any())
                {
                    _logger.LogDebug("No more failed messages older than {CutoffDate} found", cutoffDate);
                    break;
                }

                // Delete the batch
                var deletedCount = await _outboxRepository.DeleteMessagesAsync(
                    messagesToDelete.Select(m => m.Id), cancellationToken);

                totalCleaned += deletedCount;
                _logger.LogDebug("Cleaned up {DeletedCount} failed messages (total: {TotalCleaned})", 
                    deletedCount, totalCleaned);

                // If we got fewer messages than batch size, we're done
                if (messagesToDelete.Count() < batchSize)
                {
                    break;
                }

                // Small delay to prevent overwhelming the database
                await Task.Delay(100, cancellationToken);
            }

            _logger.LogInformation("Failed outbox messages cleanup completed - cleaned up {TotalCleaned} failed messages", totalCleaned);
            return totalCleaned;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during failed outbox messages cleanup process");
            throw;
        }
    }

    /// <summary>
    /// Performs comprehensive cleanup of both processed and failed messages
    /// </summary>
    /// <param name="processedRetentionDays">Retention period for processed messages (default: 7 days)</param>
    /// <param name="failedRetentionDays">Retention period for failed messages (default: 30 days)</param>
    /// <param name="batchSize">Number of messages to delete in each batch (default: 1000)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total number of messages cleaned up</returns>
    [AutomaticRetry(Attempts = 3)]
    public async Task<int> PerformFullCleanupAsync(
        int processedRetentionDays = 7,
        int failedRetentionDays = 30,
        int batchSize = 1000,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting full outbox cleanup process");

        var processedCount = await CleanupProcessedMessagesAsync(processedRetentionDays, batchSize, cancellationToken);
        var failedCount = await CleanupFailedMessagesAsync(failedRetentionDays, batchSize, cancellationToken);
        var totalCount = processedCount + failedCount;

        _logger.LogInformation("Full outbox cleanup completed - total messages cleaned: {TotalCount} (processed: {ProcessedCount}, failed: {FailedCount})", 
            totalCount, processedCount, failedCount);

        return totalCount;
    }
}
