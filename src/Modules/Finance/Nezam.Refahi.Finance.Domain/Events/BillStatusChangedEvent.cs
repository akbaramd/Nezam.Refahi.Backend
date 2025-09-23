using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a bill's status changes
/// </summary>
public class BillStatusChangedEvent : DomainEvent
{
    public Guid BillId { get; }
    public string BillNumber { get; }
    public BillStatus PreviousStatus { get; }
    public BillStatus NewStatus { get; }
    public string? Reason { get; }
    public DateTime ChangedAt { get; }
    public string? ChangedBy { get; }

    public BillStatusChangedEvent(
        Guid billId,
        string billNumber,
        BillStatus previousStatus,
        BillStatus newStatus,
        string? reason = null,
        string? changedBy = null)
    {
        BillId = billId;
        BillNumber = billNumber;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Reason = reason;
        ChangedAt = DateTime.UtcNow;
        ChangedBy = changedBy;
    }
}
