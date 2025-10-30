using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// رویداد دامنه‌ای زمانی که پرداخت منقضی می‌شود.
/// </summary>
public sealed class PaymentExpiredEvent : DomainEvent
{
  public Guid PaymentId { get; }
  public Guid BillId { get; }
  public Money Amount { get; }
  public PaymentMethod Method { get; }
  public PaymentGateway? Gateway { get; }
  public DateTime ExpiredAt { get; }
  public DateTime? OriginalExpiryDate { get; }
  public int MinutesPastExpiry { get; }

  public PaymentExpiredEvent(
    Guid paymentId,
    Guid billId,
    Money amount,
    PaymentMethod method,
    PaymentGateway? gateway = null,
    DateTime? originalExpiryDate = null)
  {
    PaymentId = paymentId;
    BillId = billId;
    Amount = amount;
    Method = method;
    Gateway = gateway;
    ExpiredAt = DateTime.UtcNow;
    OriginalExpiryDate = originalExpiryDate;

    MinutesPastExpiry = originalExpiryDate.HasValue
      ? Math.Max(0, (int)(DateTime.UtcNow - originalExpiryDate.Value).TotalMinutes)
      : 0;
  }
}
