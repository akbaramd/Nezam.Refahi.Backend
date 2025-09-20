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
    public string RequestedByNationalNumber { get; }
    public DateTime CompletedAt { get; }

    public RefundCompletedEvent(
        Guid refundId,
        Guid paymentId,
        string referenceId,
        string referenceType,
        decimal refundAmountRials,
        string requestedByNationalNumber,
        DateTime completedAt)
    {
        RefundId = refundId;
        PaymentId = paymentId;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        RefundAmountRials = refundAmountRials;
        RequestedByNationalNumber = requestedByNationalNumber;
        CompletedAt = completedAt;
    }
}