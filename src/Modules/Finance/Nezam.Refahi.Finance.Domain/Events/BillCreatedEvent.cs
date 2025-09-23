using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a new bill is created
/// </summary>
public class BillCreatedEvent : DomainEvent
{
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string Title { get; }
    public string ReferenceId { get; }
    public string BillType { get; }
    public string UserNationalNumber { get; }
    public string? UserFullName { get; }
    public Money TotalAmount { get; }
    public DateTime IssueDate { get; }
    public DateTime? DueDate { get; }
    public string? Description { get; }

    public BillCreatedEvent(
        Guid billId,
        string billNumber,
        string title,
        string referenceId,
        string billType,
        string userNationalNumber,
        string? userFullName,
        Money totalAmount,
        DateTime issueDate,
        DateTime? dueDate = null,
        string? description = null)
    {
        BillId = billId;
        BillNumber = billNumber;
        Title = title;
        ReferenceId = referenceId;
        BillType = billType;
        UserNationalNumber = userNationalNumber;
        UserFullName = userFullName;
        TotalAmount = totalAmount;
        IssueDate = issueDate;
        DueDate = dueDate;
        Description = description;
    }
}
