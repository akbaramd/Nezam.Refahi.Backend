using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a payment is created
/// This event is consumed by other modules to handle payment creation
/// </summary>
public class PaymentCreatedIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Payment details
    public Guid PaymentId { get; set; }
    public Guid BillId { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public decimal AmountRials { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? PaymentGateway { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}
