using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a payment fails
/// This event is consumed by other modules to handle payment failure
/// </summary>
public class PaymentFailedIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Payment details
    public Guid PaymentId { get; set; }
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public decimal AmountRials { get; set; }
    public DateTime FailedAt { get; set; }

    // Failure details
    public string FailureReason { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string Gateway { get; set; } = string.Empty;

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}

