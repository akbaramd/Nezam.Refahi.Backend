using MediatR;

namespace Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a tour reservation payment is requested
/// This event is consumed by the Finance module to create a bill
/// </summary>
public class ReservationPaymentRequestedIntegrationEvent : INotification
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

    // Payment details
    public decimal TotalAmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    // Bill details
    public string BillTitle { get; set; } = string.Empty;
    public string BillType { get; set; } = "TourReservation";
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();

    // Bill items
    public List<ReservationBillItemDto> BillItems { get; set; } = new();
}

/// <summary>
/// DTO for bill items in reservation payment request
/// </summary>
public class ReservationBillItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPriceRials { get; set; }
    public int Quantity { get; set; }
    public decimal? DiscountPercentage { get; set; }
}
