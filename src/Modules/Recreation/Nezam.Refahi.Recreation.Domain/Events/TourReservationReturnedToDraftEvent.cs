using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Recreation.Domain.Events;

/// <summary>
/// Domain event published when a tour reservation is returned from OnHold to Draft state
/// This indicates that capacity should be released and snapshots should be cleared
/// </summary>
public class TourReservationReturnedToDraftEvent : DomainEvent
{
    public Guid ReservationId { get; set; }
    public Guid TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public Guid? CapacityId { get; set; }
    public int ParticipantCount { get; set; }
    public DateTime ReturnedAt { get; set; }
}

