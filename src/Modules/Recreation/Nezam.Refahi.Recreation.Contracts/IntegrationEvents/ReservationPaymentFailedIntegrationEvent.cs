using MediatR;

namespace Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a reservation payment fails
/// This event is consumed by the Recreation module to handle payment failure
/// </summary>
public class ReservationPaymentFailedIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Reservation details
    public Guid ReservationId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;

    // Payment details
    public Guid? PaymentId { get; set; }
    public Guid? BillId { get; set; }
    public string? BillNumber { get; set; }
    public decimal AmountRials { get; set; }
    public DateTime FailedAt { get; set; }

    // Failure details
    public string FailureReason { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public string? GatewayTransactionId { get; set; }

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}
