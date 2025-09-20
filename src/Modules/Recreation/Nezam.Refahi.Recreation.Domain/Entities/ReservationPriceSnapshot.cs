using MCA.SharedKernel.Domain;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Represents a frozen snapshot of pricing at the time of reservation creation
/// </summary>
public sealed class ReservationPriceSnapshot : Entity<Guid>
{
    public Guid ReservationId { get; private set; }
    public ParticipantType ParticipantType { get; private set; }
    public Money BasePrice { get; private set; } = null!;
    public Money? DiscountAmount { get; private set; }
    public Money FinalPrice { get; private set; } = null!;
    public string? DiscountCode { get; private set; }
    public string? DiscountDescription { get; private set; }
    public DateTime SnapshotDate { get; private set; }
    public string? PricingRules { get; private set; } // JSON serialized rules applied
    
    // Multi-tenancy support
    public string? TenantId { get; private set; }

    // Navigation property
    public TourReservation Reservation { get; private set; } = null!;

    // Private constructor for EF Core
    private ReservationPriceSnapshot() : base() { }

    /// <summary>
    /// Creates a new price snapshot
    /// </summary>
    public ReservationPriceSnapshot(
        Guid reservationId,
        ParticipantType participantType,
        Money basePrice,
        Money finalPrice,
        Money? discountAmount = null,
        string? discountCode = null,
        string? discountDescription = null,
        string? pricingRules = null,
        string? tenantId = null)
        : base(Guid.NewGuid())
    {
        if (reservationId == Guid.Empty)
            throw new ArgumentException("Reservation ID cannot be empty", nameof(reservationId));

        ReservationId = reservationId;
        ParticipantType = participantType;
        BasePrice = basePrice ?? throw new ArgumentNullException(nameof(basePrice));
        FinalPrice = finalPrice ?? throw new ArgumentNullException(nameof(finalPrice));
        DiscountAmount = discountAmount;
        DiscountCode = discountCode?.Trim();
        DiscountDescription = discountDescription?.Trim();
        PricingRules = pricingRules?.Trim();
        TenantId = tenantId;
        SnapshotDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates discount information
    /// </summary>
    public void UpdateDiscount(Money? discountAmount, string? discountCode, string? discountDescription)
    {
        DiscountAmount = discountAmount;
        DiscountCode = discountCode?.Trim();
        DiscountDescription = discountDescription?.Trim();
        
        // Recalculate final price
        FinalPrice = BasePrice.Subtract(discountAmount ?? Money.Zero);
    }

    /// <summary>
    /// Checks if discount is applied
    /// </summary>
    public bool HasDiscount => DiscountAmount != null && DiscountAmount.AmountRials > 0;

    /// <summary>
    /// Gets the effective discount percentage
    /// </summary>
    public decimal DiscountPercentage => BasePrice.AmountRials > 0 && HasDiscount 
        ? (decimal)(DiscountAmount!.AmountRials * 100) / BasePrice.AmountRials 
        : 0;
}
