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
    /// Find wallet by national number
    /// </summary>
    /// <param name="nationalNumber">User's national number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Wallet if found, null otherwise</returns>
    Task<Wallet?> GetByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if wallet exists for national number
    /// </summary>
    /// <param name="nationalNumber">User's national number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if wallet exists, false otherwise</returns>
    Task<bool> ExistsByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default);

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
    /// <param name="nationalNumber">National number filter</param>
    /// <param name="status">Status filter</param>
    /// <param name="minBalance">Minimum balance filter</param>
    /// <param name="maxBalance">Maximum balance filter</param>
    /// <param name="createdFrom">Created from date filter</param>
    /// <param name="createdTo">Created to date filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching wallets</returns>
    Task<List<Wallet>> SearchWalletsAsync(
        string? nationalNumber = null,
        WalletStatus? status = null,
        Money? minBalance = null,
        Money? maxBalance = null,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        CancellationToken cancellationToken = default);
}