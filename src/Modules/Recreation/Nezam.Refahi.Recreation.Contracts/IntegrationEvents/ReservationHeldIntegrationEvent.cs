using MediatR;

namespace Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a tour reservation is held (moved from Draft to OnHold status)
/// This event is published after all validations pass, pricing is calculated, and snapshots are created
/// </summary>
public class ReservationHeldIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Reservation identity
    public Guid ReservationId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    
    // Tour details
    public Guid TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;
    public DateTime TourStartDate { get; set; }
    public DateTime TourEndDate { get; set; }
    
    // Reservation dates
    public DateTime ReservationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime HeldAt { get; set; } = DateTime.UtcNow;

    // User details
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserNationalCode { get; set; } = string.Empty;
    public Guid? MemberId { get; set; }

    // Reservation status
    public string Status { get; set; } = string.Empty;
    public string PreviousStatus { get; set; } = "Draft";

    // Pricing information
    public decimal TotalAmountRials { get; set; }
    public string Currency { get; set; } = "IRR";
    
    // Capacity information
    public Guid? CapacityId { get; set; }
    public string? CapacityName { get; set; }

    // Participants
    public int ParticipantCount { get; set; }
    public int MemberParticipantCount { get; set; }
    public int GuestParticipantCount { get; set; }
    public List<ReservationParticipantDto> Participants { get; set; } = new();

    // Price snapshots
    public List<ReservationPriceSnapshotDto> PriceSnapshots { get; set; } = new();

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Price snapshot information in integration events
/// </summary>
public class ReservationPriceSnapshotDto
{
    public Guid SnapshotId { get; set; }
    public string ParticipantType { get; set; } = string.Empty;
    public decimal BasePriceRials { get; set; }
    public decimal FinalPriceRials { get; set; }
    public decimal? DiscountAmountRials { get; set; }
    public string? DiscountCode { get; set; }
    public string? DiscountDescription { get; set; }
    public DateTime SnapshotDate { get; set; }
    public Guid? TourPricingId { get; set; }
}

