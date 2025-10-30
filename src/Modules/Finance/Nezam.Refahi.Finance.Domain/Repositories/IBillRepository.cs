using MCA.SharedKernel.Domain.Contracts.Repositories;
using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for bill entities
/// </summary>
public interface IBillRepository : IRepository<Bill, Guid>
{
    /// <summary>
    /// Gets bill by bill number
    /// </summary>
    Task<Bill?> GetByBillNumberAsync(string billNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bill by reference ID and type
    /// </summary>
    Task<Bill?> GetByReferenceTrackingCodeAsync(string referenceId,  CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bills by user national number
    /// </summary>
    Task<IEnumerable<Bill>> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bills by status
    /// </summary>
    Task<IEnumerable<Bill>> GetByStatusAsync(BillStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bills by bill type
    /// </summary>
    Task<IEnumerable<Bill>> GetByBillTypeAsync(string billType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overdue bills
    /// </summary>
    Task<IEnumerable<Bill>> GetOverdueBillsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bill with items
    /// </summary>
    Task<Bill?> GetWithItemsAsync(Guid billId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bill with payments
    /// </summary>
    Task<Bill?> GetWithPaymentsAsync(Guid billId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bill with refunds
    /// </summary>
    Task<Bill?> GetWithRefundsAsync(Guid billId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bill with all related data (items, payments, refunds)
    /// </summary>
    Task<Bill?> GetWithAllDataAsync(Guid billId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bills in date range
    /// </summary>
    Task<IEnumerable<Bill>> GetInDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total amount by status and date range
    /// </summary>
    Task<decimal> GetTotalAmountAsync(BillStatus status, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payment completion statistics
    /// </summary>
    Task<Dictionary<BillStatus, int>> GetStatusStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);

}