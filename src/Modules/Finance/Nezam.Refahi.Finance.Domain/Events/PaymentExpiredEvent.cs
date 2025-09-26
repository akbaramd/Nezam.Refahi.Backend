using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a payment expires
/// </summary>
public class PaymentExpiredEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public Guid ExternalUserId { get; }
    public Money Amount { get; }
    public PaymentMethod Method { get; }
    public PaymentGateway? Gateway { get; }
    public string? TrackingNumber { get; }
    public DateTime ExpiredAt { get; }
    public DateTime? OriginalExpiryDate { get; }
    public int MinutesPastExpiry { get; }

    public PaymentExpiredEvent(
        Guid paymentId,
        Guid billId,
        string billNumber,
        string referenceId,
        Guid externalUserId,
        Money amount,
        PaymentMethod method,
        PaymentGateway? gateway = null,
        string? trackingNumber = null,
        DateTime? originalExpiryDate = null)
    {
        PaymentId = paymentId;
        BillId = billId;
        BillNumber = billNumber;
        ReferenceId = referenceId;
        ExternalUserId = externalUserId;
        Amount = amount;
        Method = method;
        Gateway = gateway;
        TrackingNumber = trackingNumber;
        ExpiredAt = DateTime.UtcNow;
        OriginalExpiryDate = originalExpiryDate;
        MinutesPastExpiry = originalExpiryDate.HasValue 
            ? (int)(DateTime.UtcNow - originalExpiryDate.Value).TotalMinutes 
            : 0;
    }
}
