using Hangfire;
using Nezam.Refahi.Finance.Application.Services;

namespace Nezam.Refahi.Finance.Presentation.Services;

/// <summary>
/// Hangfire job for generating wallet snapshots
/// </summary>
public class WalletSnapshotJob
{
    private readonly IWalletSnapshotService _walletSnapshotService;
    private readonly ILogger<WalletSnapshotJob> _logger;

    public WalletSnapshotJob(
        IWalletSnapshotService walletSnapshotService,
        ILogger<WalletSnapshotJob> logger)
    {
        _walletSnapshotService = walletSnapshotService ?? throw new ArgumentNullException(nameof(walletSnapshotService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates wallet snapshots for yesterday
    /// This method will be called by Hangfire recurring jobs
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task GenerateSnapshotsForYesterdayAsync()
    {
        try
        {
            _logger.LogInformation("Starting wallet snapshot generation job");
            await _walletSnapshotService.GenerateSnapshotsForYesterdayAsync(CancellationToken.None);
            _logger.LogInformation("Wallet snapshot generation job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wallet snapshot generation job failed");
            throw;
        }
    }
}
