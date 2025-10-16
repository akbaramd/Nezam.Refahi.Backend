using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a bill is created for a reservation
/// This event is consumed by the Recreation module to update reservation status
/// </summary>
public class BillCreatedForReservationIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Bill details
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }

    // Reservation details
    public Guid ReservationId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;

    // Payment details
    public decimal TotalAmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}