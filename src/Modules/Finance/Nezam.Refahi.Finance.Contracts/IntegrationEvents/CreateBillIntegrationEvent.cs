using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event instructing Finance to create a bill.
/// Treated as a command message across bounded contexts.
/// </summary>
public class CreateBillIntegrationEvent : INotification
{
    // Envelope
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    public string TrackingCode { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty; // e.g., ReservationId or TrackingCode
    public string ReferenceType { get; set; } = "TourReservation"; // entity name for correlation

    // User
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Money
    public decimal AmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    // Bill presentation
    public string BillTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Optional meta
    public Dictionary<string, string> Metadata { get; set; } = new();

    // Optional items
    public List<CreateBillItemDto> Items { get; set; } = new();
}

public class CreateBillItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPriceRials { get; set; }
    public int Quantity { get; set; }
    public decimal? DiscountPercentage { get; set; }
}


