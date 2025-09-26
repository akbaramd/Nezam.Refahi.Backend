using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a payment is completed
/// </summary>
public class PaymentCompletedEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public Guid ExternalUserId { get; }
    public decimal AmountRials { get; }
    public string? GatewayTransactionId { get; }
    public DateTime CompletedAt { get; }

    public PaymentCompletedEvent(
        Guid paymentId,
        string referenceId,
        string referenceType,
        Guid externalUserId,
        decimal amountRials,
        string? gatewayTransactionId,
        DateTime completedAt)
    {
        PaymentId = paymentId;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        ExternalUserId = externalUserId;
        AmountRials = amountRials;
        GatewayTransactionId = gatewayTransactionId;
        CompletedAt = completedAt;
    }
}