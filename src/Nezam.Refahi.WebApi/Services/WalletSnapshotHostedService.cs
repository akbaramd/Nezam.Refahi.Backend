using Hangfire;
using Nezam.Refahi.Finance.Application.Services;

namespace Nezam.Refahi.WebApi.Services;

/// <summary>
/// Hosted service for managing wallet snapshot recurring jobs
/// </summary>
public sealed class WalletSnapshotHostedService : IHostedService
{
    private readonly ILogger<WalletSnapshotHostedService> _logger;
    private readonly IWalletSnapshotService _walletSnapshotService;

    public WalletSnapshotHostedService(
        ILogger<WalletSnapshotHostedService> logger,
        IWalletSnapshotService walletSnapshotService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _walletSnapshotService = walletSnapshotService ?? throw new ArgumentNullException(nameof(walletSnapshotService));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting WalletSnapshotHostedService");

        try
        {
          await _walletSnapshotService.GenerateSnapshotsForYesterdayAsync(CancellationToken.None);
            // Schedule daily wallet snapshot job at 2:00 AM UTC
            RecurringJob.AddOrUpdate(
                "wallet-snapshot-daily",
                () => _walletSnapshotService.GenerateSnapshotsForYesterdayAsync(CancellationToken.None),
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
