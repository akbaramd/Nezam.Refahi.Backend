using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a refund is completed
/// </summary>
public class RefundCompletedEvent : DomainEvent
{
    public Guid RefundId { get; }
    public Guid PaymentId { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public decimal RefundAmountRials { get; }
    public Guid RequestedByExternalUserId { get; }
    public DateTime CompletedAt { get; }

    public RefundCompletedEvent(
        Guid refundId,
        Guid paymentId,
        string referenceId,
        string referenceType,
        decimal refundAmountRials,
        Guid requestedByExternalUserId,
        DateTime completedAt)
    {
        RefundId = refundId;
        PaymentId = paymentId;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        RefundAmountRials = refundAmountRials;
        RequestedByExternalUserId = requestedByExternalUserId;
        CompletedAt = completedAt;
    }
}