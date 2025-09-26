using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for WalletSnapshot aggregate
/// </summary>
public interface IWalletSnapshotRepository : IRepository<WalletSnapshot, Guid>
{
    /// <summary>
    /// Get snapshots by wallet ID
    /// </summary>
    Task<IEnumerable<WalletSnapshot>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get snapshots by external user ID
    /// </summary>
    Task<IEnumerable<WalletSnapshot>> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the latest snapshot for a wallet
    /// </summary>
    Task<WalletSnapshot?> GetLatestByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the latest snapshot for a user
    /// </summary>
    Task<WalletSnapshot?> GetLatestByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get snapshot for a specific date
    /// </summary>
    Task<WalletSnapshot?> GetByWalletIdAndDateAsync(Guid walletId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get snapshots within date range
    /// </summary>
    Task<IEnumerable<WalletSnapshot>> GetByWalletIdAndDateRangeAsync(
        Guid walletId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get snapshots by user and date range
    /// </summary>
    Task<IEnumerable<WalletSnapshot>> GetByUserAndDateRangeAsync(
        Guid externalUserId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if snapshot exists for a specific date
    /// </summary>
    Task<bool> ExistsByWalletIdAndDateAsync(Guid walletId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get snapshots that need to be created (missing snapshots)
    /// </summary>
    Task<IEnumerable<Guid>> GetWalletsWithoutSnapshotForDateAsync(DateTime date, CancellationToken cancellationToken = default);
}
