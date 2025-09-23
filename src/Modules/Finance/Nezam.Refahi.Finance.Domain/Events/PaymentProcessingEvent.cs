using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a payment starts processing
/// </summary>
public class PaymentProcessingEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public string UserNationalNumber { get; }
    public Money Amount { get; }
    public PaymentGateway Gateway { get; }
    public string? GatewayTransactionId { get; }
    public string? TrackingNumber { get; }
    public DateTime ProcessingStartedAt { get; }
    public string? CallbackUrl { get; }

    public PaymentProcessingEvent(
        Guid paymentId,
        Guid billId,
        string billNumber,
        string referenceId,
        string userNationalNumber,
        Money amount,
        PaymentGateway gateway,
        string? gatewayTransactionId = null,
        string? trackingNumber = null,
        string? callbackUrl = null)
    {
        PaymentId = paymentId;
        BillId = billId;
        BillNumber = billNumber;
        ReferenceId = referenceId;
        UserNationalNumber = userNationalNumber;
        Amount = amount;
        Gateway = gateway;
        GatewayTransactionId = gatewayTransactionId;
        TrackingNumber = trackingNumber;
        ProcessingStartedAt = DateTime.UtcNow;
        CallbackUrl = callbackUrl;
    }
}
