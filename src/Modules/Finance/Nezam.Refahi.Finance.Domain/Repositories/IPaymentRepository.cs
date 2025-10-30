using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for payment entities
/// </summary>
public interface IPaymentRepository : IRepository<Payment, Guid>
{
    /// <summary>
    /// Gets payments by bill ID
    /// </summary>
    Task<IEnumerable<Payment>> GetByBillIdAsync(Guid billId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payments by status
    /// </summary>
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payments by gateway
    /// </summary>
    Task<IEnumerable<Payment>> GetByGatewayAsync(PaymentGateway gateway, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payment by gateway transaction ID
    /// </summary>
    Task<Payment?> GetByGatewayTransactionIdAsync(string gatewayTransactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payment by tracking number
    /// </summary>
    Task<Payment?> GetByGatewayReferenceAsync(string trackingNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired payments
    /// </summary>
    Task<IEnumerable<Payment>> GetExpiredPaymentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payments with transactions
    /// </summary>
    Task<Payment?> GetWithTransactionsAsync(Guid paymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payments in date range
    /// </summary>
    Task<IEnumerable<Payment>> GetInDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total amount by status and date range
    /// </summary>
    Task<decimal> GetTotalAmountAsync(PaymentStatus status, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
}