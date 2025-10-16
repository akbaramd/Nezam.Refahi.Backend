using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.Extensions.Logging;

namespace Nezam.Refahi.Shared.Infrastructure.Services;

/// <summary>
/// Service for Hangfire maintenance operations including failed job cleanup
/// </summary>
public class HangfireMaintenanceService
{
    private readonly ILogger<HangfireMaintenanceService> _logger;

    public HangfireMaintenanceService(ILogger<HangfireMaintenanceService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Purges failed jobs older than the specified age
    /// </summary>
    /// <param name="maxAge">Maximum age of failed jobs to retain</param>
    /// <returns>Number of jobs purged</returns>
    [AutomaticRetry(Attempts = 3)]
    public async Task<int> PurgeFailedJobsOlderThanAsync(TimeSpan maxAge)
    {
        try
        {
            await Task.CompletedTask;
            _logger.LogInformation("Starting purge of failed jobs older than {MaxAge}", maxAge);

            var api = JobStorage.Current.GetMonitoringApi();
            var cutoff = DateTime.UtcNow - maxAge;
            var purgedCount = 0;

            // Page through failures
            const int pageSize = 1000;
            int from = 0;
            IList<KeyValuePair<string, FailedJobDto>> batch;

            do
            {
                var page = api.FailedJobs(from, pageSize);
                batch = page.ToList();

                foreach (var kv in batch)
                {
                    var id = kv.Key;
                    var failedAt = kv.Value?.FailedAt;

                    if (failedAt.HasValue && failedAt.Value.ToUniversalTime() < cutoff)
                    {
                        try
                        {
                            BackgroundJob.Delete(id);
                            purgedCount++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete job {JobId}", id);
                        }
                    }
                }

                from += pageSize;
            } while (batch.Count == pageSize);

            _logger.LogInformation("Purged {PurgedCount} failed jobs older than {MaxAge}", purgedCount, maxAge);
            return purgedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while purging failed jobs");
            throw;
        }
    }

    /// <summary>
    /// Purges old successful jobs to keep storage clean
    /// Note: This is mainly for manual cleanup as successful jobs auto-expire based on WithJobExpirationTimeout
    /// </summary>
    /// <param name="maxAge">Maximum age of successful jobs to retain</param>
    /// <returns>Number of jobs purged</returns>
    [AutomaticRetry(Attempts = 3)]
    public async Task<int> PurgeOldSuccessfulJobsAsync(TimeSpan maxAge) 
    {
        try
        {
            await Task.CompletedTask;
            _logger.LogInformation("Starting purge of old successful jobs older than {MaxAge}", maxAge);

            var api = JobStorage.Current.GetMonitoringApi();
            var cutoff = DateTime.UtcNow - maxAge;
            var purgedCount = 0;

            // Page through succeeded jobs
            const int pageSize = 1000;
            int from = 0;
            IList<KeyValuePair<string, SucceededJobDto>> batch;

            do
            {
                var page = api.SucceededJobs(from, pageSize);
                batch = page.ToList();

                foreach (var kv in batch)
                {
                    var id = kv.Key;
                    var succeededAt = kv.Value?.SucceededAt;

                    if (succeededAt.HasValue && succeededAt.Value.ToUniversalTime() < cutoff)
                    {
                        try
                        {
                            BackgroundJob.Delete(id);
                            purgedCount++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete successful job {JobId}", id);
                        }
                    }
                }

                from += pageSize;
            } while (batch.Count == pageSize);

            _logger.LogInformation("Purged {PurgedCount} old successful jobs older than {MaxAge}", purgedCount, maxAge);
            return purgedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while purging old successful jobs");
            throw;
        }
    }

    /// <summary>
    /// Gets Hangfire statistics for monitoring
    /// </summary>
    /// <returns>Hangfire statistics</returns>
    public async Task<HangfireStatistics> GetStatisticsAsync()
    {
        try
        {
            await Task.CompletedTask;
            var api = JobStorage.Current.GetMonitoringApi();
            var stats = api.GetStatistics();

            return new HangfireStatistics
            {
                Enqueued = stats.Enqueued,
                Failed = stats.Failed,
                Processing = stats.Processing,
                Scheduled = stats.Scheduled,
                Succeeded = stats.Succeeded,
                Deleted = stats.Deleted,
                Servers = stats.Servers,
                Recurring = stats.Recurring,
                Retries = stats.Retries ?? 0,
                Awaiting = stats.Awaiting ?? 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting Hangfire statistics");
            throw;
        }
    }
}

/// <summary>
/// Hangfire statistics DTO
/// </summary>
public class HangfireStatistics
{
    public long Enqueued { get; set; }
    public long Failed { get; set; }
    public long Processing { get; set; }
    public long Scheduled { get; set; }
    public long Succeeded { get; set; }
    public long Deleted { get; set; }
    public long Servers { get; set; }
    public long Recurring { get; set; }
    public long Retries { get; set; }
    public long Awaiting { get; set; }
}
