using MediatR;

namespace Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a tour reservation is cancelled
/// This event can be consumed by other modules for notifications, refunds, etc.
/// </summary>
public class ReservationCancelledIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Reservation details
    public Guid ReservationId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public Guid TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public DateTime CancelledAt { get; set; }

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserNationalCode { get; set; } = string.Empty;

    // Cancellation details
    public string CancellationReason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool WasDeleted { get; set; }

    // Financial details
    public decimal? RefundableAmountRials { get; set; }
    public decimal? PaidAmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    // Participants
    public int ParticipantCount { get; set; }

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}
