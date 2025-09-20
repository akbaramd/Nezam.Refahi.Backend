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
    public string UserNationalNumber { get; }
    public decimal AmountRials { get; }
    public string? GatewayTransactionId { get; }
    public DateTime CompletedAt { get; }

    public PaymentCompletedEvent(
        Guid paymentId,
        string referenceId,
        string referenceType,
        string userNationalNumber,
        decimal amountRials,
        string? gatewayTransactionId,
        DateTime completedAt)
    {
        PaymentId = paymentId;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        UserNationalNumber = userNationalNumber;
        AmountRials = amountRials;
        GatewayTransactionId = gatewayTransactionId;
        CompletedAt = completedAt;
    }
}