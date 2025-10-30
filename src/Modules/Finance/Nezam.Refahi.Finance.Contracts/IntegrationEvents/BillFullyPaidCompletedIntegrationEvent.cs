using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a bill is fully paid and completed
/// This event is consumed by other modules to handle successful payment completion
/// </summary>
public class BillFullyPaidCompletedIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Bill details
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; } = Guid.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public string ReferenceTrackingCode { get; set; } = string.Empty;
    public decimal PaidAmountRials { get; set; }
    public DateTime PaidAt { get; set; }

    // Payment details
    public Guid PaymentId { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReference { get; set; }
    public string Gateway { get; set; } = string.Empty;

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}
