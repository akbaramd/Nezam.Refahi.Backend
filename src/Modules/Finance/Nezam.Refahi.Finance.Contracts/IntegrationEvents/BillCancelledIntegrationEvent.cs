using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a bill is cancelled
/// This event is consumed by other modules to handle bill cancellation
/// </summary>
public class BillCancelledIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Bill details
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CancelledAt { get; set; }

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}