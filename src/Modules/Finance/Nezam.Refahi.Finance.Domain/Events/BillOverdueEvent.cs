using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a bill becomes overdue
/// </summary>
public class BillOverdueEvent : DomainEvent
{
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public Guid ExternalUserId { get; }
    public Money TotalAmount { get; }
    public Money RemainingAmount { get; }
    public DateTime DueDate { get; }
    public DateTime OverdueDate { get; }
    public int DaysOverdue { get; }

    public BillOverdueEvent(
        Guid billId,
        string billNumber,
        string referenceId,
        string referenceType,
        Guid externalUserId,  
        Money totalAmount,
        Money remainingAmount,
        DateTime dueDate,
        DateTime overdueDate)
    {
        BillId = billId;
        BillNumber = billNumber;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        ExternalUserId = externalUserId;
        TotalAmount = totalAmount;
        RemainingAmount = remainingAmount;
        DueDate = dueDate;
        OverdueDate = overdueDate;
        DaysOverdue = (int)(overdueDate - dueDate).TotalDays;
    }
}
