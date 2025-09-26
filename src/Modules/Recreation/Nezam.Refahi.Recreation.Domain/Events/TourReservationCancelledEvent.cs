using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Recreation.Domain.Events;

/// <summary>
/// Domain event published when a tour reservation is cancelled
/// </summary>
public class TourReservationCancelledEvent : DomainEvent    
{
    public Guid ReservationId { get; set; }
    public Guid TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public string CancellationReason { get; set; } = string.Empty;
    public DateTime CancelledAt { get; set; }
    public decimal RefundAmount { get; set; }
    public bool RefundProcessed { get; set; }
}
