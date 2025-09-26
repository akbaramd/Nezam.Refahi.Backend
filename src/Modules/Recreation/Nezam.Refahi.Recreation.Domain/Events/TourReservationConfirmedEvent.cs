using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Recreation.Domain.Events;

/// <summary>
/// Domain event published when a tour reservation is confirmed
/// </summary>
public class TourReservationConfirmedEvent : DomainEvent
{
    public Guid ReservationId { get; set; }
    public Guid TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public DateTime TourStartDate { get; set; }
    public DateTime TourEndDate { get; set; }
    public int ParticipantCount { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime ConfirmedAt { get; set; }
}
