using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a payment fails
/// </summary>
public class PaymentFailedEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public Guid ExternalUserId { get; }
    public string? FailureReason { get; }
    public string? ErrorCode { get; }

    public PaymentFailedEvent(
        Guid paymentId,
        string referenceId,
        string referenceType,
        Guid externalUserId,
        string? failureReason,
        string? errorCode = null)
    {
        PaymentId = paymentId;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        ExternalUserId = externalUserId;
        FailureReason = failureReason;
        ErrorCode = errorCode;
    }
}