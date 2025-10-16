using MediatR;

namespace Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a reservation payment is completed
/// This event is consumed by the Recreation module to update reservation status
/// </summary>
public class ReservationPaymentCompletedIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Reservation details
    public Guid ReservationId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;

    // Payment details
    public Guid PaymentId { get; set; }
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public decimal AmountRials { get; set; }
    public DateTime PaidAt { get; set; }

    // Gateway details
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReference { get; set; }
    public string Gateway { get; set; } = string.Empty;

    // User details
    public Guid ExternalUserId { get; set; }

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}
