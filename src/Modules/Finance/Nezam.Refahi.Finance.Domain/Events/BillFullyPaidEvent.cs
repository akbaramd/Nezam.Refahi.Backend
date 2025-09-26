using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a bill is fully paid
/// </summary>
public class BillFullyPaidEvent : DomainEvent
{
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public Guid ExternalUserId { get; }
    public Money TotalAmount { get; }
    public Money PaidAmount { get; }
    public DateTime FullyPaidDate { get; }
    public int PaymentCount { get; }
    public string? LastPaymentMethod { get; }
    public string? LastGateway { get; }

    public BillFullyPaidEvent(
        Guid billId,
        string billNumber,
        string referenceId,
        string referenceType,
        Guid externalUserId,
        Money totalAmount,
        Money paidAmount,
        DateTime fullyPaidDate,
        int paymentCount,
        string? lastPaymentMethod = null,
        string? lastGateway = null)
    {
        BillId = billId;
        BillNumber = billNumber;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        ExternalUserId = externalUserId;
        TotalAmount = totalAmount;
        PaidAmount = paidAmount;
        FullyPaidDate = fullyPaidDate;
        PaymentCount = paymentCount;
        LastPaymentMethod = lastPaymentMethod;
        LastGateway = lastGateway;
    }
}
