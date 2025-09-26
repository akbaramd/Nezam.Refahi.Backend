namespace Nezam.Refahi.Finance.Application.Services;

/// <summary>
/// Service interface for generating wallet snapshots
/// </summary>
public interface IWalletSnapshotService
{
    /// <summary>
    /// Generate snapshots for all wallets for a specific date
    /// </summary>
    /// <param name="snapshotDate">Date to generate snapshots for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of snapshots created</returns>
    Task<int> GenerateSnapshotsForDateAsync(DateTime snapshotDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate snapshot for a specific wallet and date
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <param name="snapshotDate">Date to generate snapshot for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task GenerateSnapshotForWalletAsync(Guid walletId, DateTime snapshotDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate snapshots for all wallets for yesterday (for daily batch job)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of snapshots created</returns>
    Task<int> GenerateSnapshotsForYesterdayAsync(CancellationToken cancellationToken = default);
}
