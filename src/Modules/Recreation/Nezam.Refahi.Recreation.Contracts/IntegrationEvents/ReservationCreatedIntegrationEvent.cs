using MediatR;

namespace Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a tour reservation is created
/// This event can be consumed by other modules for notifications, analytics, etc.
/// </summary>
public class ReservationCreatedIntegrationEvent : INotification
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
    public DateTime? ExpiryDate { get; set; }

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserNationalCode { get; set; } = string.Empty;

    // Reservation status and pricing
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    // Participants
    public int ParticipantCount { get; set; }
    public List<ReservationParticipantDto> Participants { get; set; } = new();

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for reservation participants in integration events
/// </summary>
public class ReservationParticipantDto
{
    public string Name { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsMainParticipant { get; set; }
    public int Age { get; set; }
}
