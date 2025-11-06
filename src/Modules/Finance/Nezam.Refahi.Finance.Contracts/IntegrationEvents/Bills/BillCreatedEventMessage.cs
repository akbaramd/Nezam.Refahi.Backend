using MediatR;

namespace Nezam.Refahi.Contracts.Finance.v1.Messages;

/// <summary>
/// Integration event published when a bill is created
/// </summary>
public class BillCreatedEventMessage : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Bill details
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }

    // Reference details
    public string TrackingCode { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; } = Guid.Empty;
    public string ReferenceType { get; set; } = string.Empty;

    // Payment details
    public decimal TotalAmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}



