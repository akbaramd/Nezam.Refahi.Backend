using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a refund is initiated
/// </summary>
public class RefundInitiatedEvent : DomainEvent
{
    public Guid RefundId { get; }
    public Guid PaymentId { get; }
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public Money RefundAmount { get; }
    public Guid RequestedByExternalUserId { get; }
    public string Reason { get; }
    public DateTime InitiatedAt { get; }
    public string? RequestedBy { get; }
    public string? Notes { get; }

    public RefundInitiatedEvent(
        Guid refundId,
        Guid paymentId,
        Guid billId,
        string billNumber,
        string referenceId,
        string referenceType,
        Money refundAmount,
        Guid requestedByExternalUserId,
        string reason,
        string? requestedBy = null,
        string? notes = null)
    {
        RefundId = refundId;
        PaymentId = paymentId;
        BillId = billId;
        BillNumber = billNumber;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        RefundAmount = refundAmount;
        RequestedByExternalUserId = requestedByExternalUserId;
        Reason = reason;
        InitiatedAt = DateTime.UtcNow;
        RequestedBy = requestedBy;
        Notes = notes;
    }
}
