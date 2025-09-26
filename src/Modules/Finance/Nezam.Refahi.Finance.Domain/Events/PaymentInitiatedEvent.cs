using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a payment is initiated
/// </summary>
public class PaymentInitiatedEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public Guid ExternalUserId { get; }
    public Money Amount { get; }
    public PaymentMethod Method { get; }
    public PaymentGateway? Gateway { get; }
    public string? TrackingNumber { get; }
    public DateTime InitiatedAt { get; }
    public DateTime? ExpiryDate { get; }
    public string? Description { get; }

    public PaymentInitiatedEvent(
        Guid paymentId,
        Guid billId,
        string billNumber,
        string referenceId,
        string referenceType,
        Guid externalUserId,
        Money amount,
        PaymentMethod method,
        PaymentGateway? gateway = null,
        string? trackingNumber = null,
        DateTime? expiryDate = null,
        string? description = null)
    {
        PaymentId = paymentId;
        BillId = billId;
        BillNumber = billNumber;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        ExternalUserId = externalUserId;
        Amount = amount;
        Method = method;
        Gateway = gateway;
        TrackingNumber = trackingNumber;
        InitiatedAt = DateTime.UtcNow;
        ExpiryDate = expiryDate;
        Description = description;
    }
}
