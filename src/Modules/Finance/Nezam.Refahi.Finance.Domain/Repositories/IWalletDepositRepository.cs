using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for WalletDeposit aggregate
/// </summary>
public interface IWalletDepositRepository : IRepository<WalletDeposit, Guid>
{
    /// <summary>
    /// Get deposits by wallet ID
    /// </summary>
    Task<IEnumerable<WalletDeposit>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get deposits by tracking code
    /// </summary>
    Task<WalletDeposit?> GetByTrackingCodeAsync(string trackingCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get deposits by user national number
    /// </summary>
    /// <param name="externalUserId">User's external ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of deposits</returns>
    Task<IEnumerable<WalletDeposit>> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default);  

    /// <summary>
    /// Get deposits by status
    /// </summary>
    /// <param name="status">Status of deposits</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of deposits</returns>
    /// </summary>
    Task<IEnumerable<WalletDeposit>> GetByStatusAsync(WalletDepositStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get deposits by external reference
    /// </summary>
    Task<WalletDeposit?> GetByExternalReferenceAsync(string externalReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending deposits for a wallet
    /// </summary>
    Task<IEnumerable<WalletDeposit>> GetPendingDepositsAsync(Guid walletId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get deposits within date range
    /// </summary>
    Task<IEnumerable<WalletDeposit>> GetDepositsByDateRangeAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get deposits by user and date range
    /// </summary>
    Task<IEnumerable<WalletDeposit>> GetDepositsByUserAndDateRangeAsync(
        Guid externalUserId,
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);
}
