using Hangfire;

namespace Nezam.Refahi.Finance.Presentation.Services;

/// <summary>
/// Hosted service for managing wallet snapshot recurring jobs
/// </summary>
public sealed class WalletSnapshotHostedService : IHostedService
{
    private readonly ILogger<WalletSnapshotHostedService> _logger;

    public WalletSnapshotHostedService(ILogger<WalletSnapshotHostedService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting WalletSnapshotHostedService");
        await Task.CompletedTask;
        try
        {
            // Schedule daily wallet snapshot job at 2:00 AM UTC
            RecurringJob.AddOrUpdate<WalletSnapshotJob>(
                "wallet-snapshot-daily",
                job => job.GenerateSnapshotsForYesterdayAsync(),
                "0 2 * * *");

            _logger.LogInformation("Wallet snapshot recurring job scheduled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule wallet snapshot recurring job");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping WalletSnapshotHostedService");
        return Task.CompletedTask;
    }
}
