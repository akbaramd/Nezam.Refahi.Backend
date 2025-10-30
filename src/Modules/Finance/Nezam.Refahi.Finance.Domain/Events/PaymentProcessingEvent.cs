using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// رویداد دامنه‌ای زمانی که پرداخت وارد وضعیت در حال پردازش می‌شود.
/// </summary>
public sealed class PaymentProcessingEvent : DomainEvent
{
  public Guid PaymentId { get; }
  public Guid BillId { get; }
  public Money Amount { get; }
  public PaymentGateway? Gateway { get; }
  public string? GatewayTransactionId { get; }
  public DateTime ProcessingStartedAt { get; }

  public PaymentProcessingEvent(
    Guid paymentId,
    Guid billId,
    Money amount,
    PaymentGateway? gateway,
    string? gatewayTransactionId = null)
  {
    PaymentId = paymentId;
    BillId = billId;
    Amount = amount;
    Gateway = gateway;
    GatewayTransactionId = gatewayTransactionId;
    ProcessingStartedAt = DateTime.UtcNow;
  }
}
