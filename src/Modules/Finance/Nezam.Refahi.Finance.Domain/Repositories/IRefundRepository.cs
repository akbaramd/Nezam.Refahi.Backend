using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for refund entities
/// </summary>
public interface IRefundRepository : IRepository<Refund, Guid>
{
    /// <summary>
    /// Gets refunds by bill ID
    /// </summary>
    Task<IEnumerable<Refund>> GetByBillIdAsync(Guid billId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets refunds by status
    /// </summary>
    Task<IEnumerable<Refund>> GetByStatusAsync(RefundStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending refunds for review
    /// </summary>
    Task<IEnumerable<Refund>> GetPendingRefundsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets refunds by requester national number
    /// </summary>
    Task<IEnumerable<Refund>> GetByRequesterAsync(string nationalNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets refunds in date range
    /// </summary>
    Task<IEnumerable<Refund>> GetInDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total refund amount by status and date range
    /// </summary>
    Task<decimal> GetTotalRefundAmountAsync(RefundStatus status, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
}