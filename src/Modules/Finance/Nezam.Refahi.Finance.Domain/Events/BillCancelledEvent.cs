using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a bill is cancelled
/// </summary>
public class BillCancelledEvent : DomainEvent
{
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public Guid ExternalUserId { get; }
    public Money TotalAmount { get; }
    public Money PaidAmount { get; }
    public Money RefundAmount { get; }
    public string Reason { get; }
    public DateTime CancelledAt { get; }
    public string? CancelledBy { get; }
    public bool RequiresRefund { get; }

    public BillCancelledEvent(
        Guid billId,
        string billNumber,
        string referenceId,
        string referenceType,
        Guid externalUserId,
        Money totalAmount,
        Money paidAmount,
        Money refundAmount,
        string reason,
        string? cancelledBy = null)
    {
        BillId = billId;
        BillNumber = billNumber;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        ExternalUserId = externalUserId;
        TotalAmount = totalAmount;
        PaidAmount = paidAmount;
        RefundAmount = refundAmount;
        Reason = reason;
        CancelledAt = DateTime.UtcNow;
        CancelledBy = cancelledBy;
        RequiresRefund = refundAmount.AmountRials > 0;
    }
}
