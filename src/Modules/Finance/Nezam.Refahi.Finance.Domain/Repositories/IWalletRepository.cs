using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for Wallet aggregate
/// </summary>
public interface IWalletRepository : IRepository<Wallet, Guid>
{
    /// <summary>
    /// Find wallet by external user ID
    /// </summary>
    /// <param name="externalUserId">User's external ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Wallet if found, null otherwise</returns>
    Task<Wallet?> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if wallet exists for external user ID
    /// </summary>
    /// <param name="externalUserId">User's external ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if wallet exists, false otherwise</returns>
    Task<bool> ExistsByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get wallets by status
    /// </summary>
    /// <param name="status">Wallet status to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of wallets with specified status</returns>
    Task<List<Wallet>> GetByStatusAsync(WalletStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get wallets with balance greater than specified amount
    /// </summary>
    /// <param name="minimumBalance">Minimum balance threshold</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of wallets with sufficient balance</returns>
    Task<List<Wallet>> GetWithMinimumBalanceAsync(Money minimumBalance, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active wallets
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active wallets</returns>
    Task<List<Wallet>> GetActiveWalletsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get wallets with recent activity
    /// </summary>
    /// <param name="days">Number of days to look back</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of wallets with recent activity</returns>
    Task<List<Wallet>> GetWalletsWithRecentActivityAsync(int days = 7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get wallets by balance range
    /// </summary>
    /// <param name="minBalance">Minimum balance</param>
    /// <param name="maxBalance">Maximum balance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of wallets within balance range</returns>
    Task<List<Wallet>> GetWalletsByBalanceRangeAsync(Money minBalance, Money maxBalance, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total balance of all active wallets
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total balance</returns>
    Task<decimal> GetTotalWalletBalanceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get wallet status statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of wallet statuses and their counts</returns>
    Task<Dictionary<WalletStatus, int>> GetWalletStatusStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Search wallets with multiple criteria
    /// </summary>
    /// <param name="externalUserId">External user ID filter</param>
    /// <param name="status">Status filter</param>
    /// <param name="minBalance">Minimum balance filter</param>
    /// <param name="maxBalance">Maximum balance filter</param>
    /// <param name="createdFrom">Created from date filter</param>
    /// <param name="createdTo">Created to date filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching wallets</returns>
    Task<List<Wallet>> SearchWalletsAsync(
        Guid? externalUserId = null,
        WalletStatus? status = null,
        Money? minBalance = null,
        Money? maxBalance = null,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        CancellationToken cancellationToken = default);

    // Snapshot and Balance Management Methods

    /// <summary>
    /// Get wallets that don't have snapshots for a specific date
    /// </summary>
    /// <param name="date">Date to check for snapshots</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of wallet IDs without snapshots</returns>
    Task<IEnumerable<Guid>> GetWalletsWithoutSnapshotForDateAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the latest snapshot for a wallet
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Latest snapshot or null</returns>
    Task<WalletSnapshot?> GetLatestSnapshotAsync(Guid walletId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions after a specific snapshot date
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <param name="snapshotDate">Snapshot date</param>
    /// <param name="targetDate">Target date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transactions after snapshot date</returns>
    Task<IEnumerable<WalletTransaction>> GetTransactionsAfterSnapshotAsync(
        Guid walletId, 
        DateTime? snapshotDate, 
        DateTime targetDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate wallet balance from snapshots and transactions
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <param name="asOfDate">Date to calculate balance for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Calculated balance</returns>
    Task<Money> CalculateWalletBalanceAsync(Guid walletId, DateTime? asOfDate = null, CancellationToken cancellationToken = default);


    /// <summary>
    /// Get wallet by external user ID with refreshed balance
    /// </summary>
    /// <param name="externalUserId">User external ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Wallet with refreshed balance</returns>
    Task<Wallet?> GetByExternalUserIdWithRefreshedBalanceAsync(Guid externalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get wallet by ID with refreshed balance
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Wallet with refreshed balance</returns>
    Task<Wallet?> GetByIdWithRefreshedBalanceAsync(Guid walletId, CancellationToken cancellationToken = default);
}