using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Recreation.Domain.Events;

/// <summary>
/// Domain event published when a tour reservation is created
/// </summary>
public class TourReservationCreatedEvent : DomainEvent
{
    public Guid ReservationId { get; set; }
    public Guid TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime TourStartDate { get; set; }
    public DateTime TourEndDate { get; set; }
    public int ParticipantCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
}
