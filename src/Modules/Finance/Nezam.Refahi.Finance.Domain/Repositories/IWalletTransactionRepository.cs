using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for WalletTransaction entity
/// </summary>
public interface IWalletTransactionRepository : IRepository<WalletTransaction, Guid>
{
    /// <summary>
    /// Retrieves transactions for a specific wallet with pagination.
    /// </summary>
    /// <param name="walletId">The ID of the wallet.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of wallet transactions.</returns>
    Task<List<WalletTransaction>> GetByWalletIdAsync(Guid walletId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves transactions for a specific wallet within a date range.
    /// </summary>
    /// <param name="walletId">The ID of the wallet.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of wallet transactions.</returns>
    Task<List<WalletTransaction>> GetByWalletIdAndDateRangeAsync(Guid walletId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves transactions by type for a specific wallet.
    /// </summary>
    /// <param name="walletId">The ID of the wallet.</param>
    /// <param name="transactionType">The type of transaction.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of wallet transactions.</returns>
    Task<List<WalletTransaction>> GetByWalletIdAndTypeAsync(Guid walletId, WalletTransactionType transactionType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the latest transaction for a specific wallet.
    /// </summary>
    /// <param name="walletId">The ID of the wallet.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The latest transaction or null if none exists.</returns>
    Task<WalletTransaction?> GetLatestByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves transactions by reference ID.
    /// </summary>
    /// <param name="referenceId">The reference ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of wallet transactions.</returns>
    Task<List<WalletTransaction>> GetByReferenceIdAsync(string referenceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves transactions by external reference.
    /// </summary>
    /// <param name="externalReference">The external reference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of wallet transactions.</returns>
    Task<List<WalletTransaction>> GetByExternalReferenceAsync(string externalReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts transactions for a specific wallet.
    /// </summary>
    /// <param name="walletId">The ID of the wallet.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The count of transactions.</returns>
    Task<int> CountByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts transactions for a specific wallet within a date range.
    /// </summary>
    /// <param name="walletId">The ID of the wallet.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The count of transactions.</returns>
    Task<int> CountByWalletIdAndDateRangeAsync(Guid walletId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets transaction statistics for a specific wallet.
    /// </summary>
    /// <param name="walletId">The ID of the wallet.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of transaction types and their counts.</returns>
    Task<Dictionary<WalletTransactionType, int>> GetTransactionTypeStatisticsAsync(Guid walletId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the balance history for a specific wallet.
    /// </summary>
    /// <param name="walletId">The ID of the wallet.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of balance history points.</returns>
    Task<List<(DateTime Date, decimal Balance)>> GetBalanceHistoryAsync(Guid walletId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
}
