using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a refund fails
/// </summary>
public class RefundFailedEvent : DomainEvent
{
    public Guid RefundId { get; }
    public Guid PaymentId { get; }
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public Money RefundAmount { get; }
    public string RequestedByNationalNumber { get; }
    public string FailureReason { get; }
    public string? ErrorCode { get; }
    public DateTime FailedAt { get; }
    public string? ProcessedBy { get; }

    public RefundFailedEvent(
        Guid refundId,
        Guid paymentId,
        Guid billId,
        string billNumber,
        string referenceId,
        Money refundAmount,
        string requestedByNationalNumber,
        string failureReason,
        string? errorCode = null,
        string? processedBy = null)
    {
        RefundId = refundId;
        PaymentId = paymentId;
        BillId = billId;
        BillNumber = billNumber;
        ReferenceId = referenceId;
        RefundAmount = refundAmount;
        RequestedByNationalNumber = requestedByNationalNumber;
        FailureReason = failureReason;
        ErrorCode = errorCode;
        FailedAt = DateTime.UtcNow;
        ProcessedBy = processedBy;
    }
}
