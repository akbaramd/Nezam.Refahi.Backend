using MCA.SharedKernel.Domain;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Entity representing pricing rules for different participant types in a tour
/// </summary>
public sealed class TourPricing : Entity<Guid>
{
    public Guid TourId { get; private set; }
    public ParticipantType ParticipantType { get; private set; }
    public Money Price { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public bool IsActive { get; private set; }
    public int? MinQuantity { get; private set; }
    public int? MaxQuantity { get; private set; }
    public decimal? DiscountPercentage { get; private set; }

    // Navigation property
    public Tour Tour { get; private set; } = null!;

    // Private constructor for EF Core
    private TourPricing() : base() { }

    /// <summary>
    /// Creates a new tour pricing rule
    /// </summary>
    public TourPricing(
        Guid tourId,
        ParticipantType participantType,
        Money price,
        string? description = null,
        DateTime? validFrom = null,
        DateTime? validTo = null,
        int? minQuantity = null,
        int? maxQuantity = null,
        decimal? discountPercentage = null)
        : base(Guid.NewGuid())
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID cannot be empty", nameof(tourId));

        ValidateQuantities(minQuantity, maxQuantity);
        ValidateDiscount(discountPercentage);
        ValidateDates(validFrom, validTo);

        TourId = tourId;
        ParticipantType = participantType;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Description = description?.Trim();
        ValidFrom = validFrom;
        ValidTo = validTo;
        IsActive = true;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
        DiscountPercentage = discountPercentage;
    }

    /// <summary>
    /// Updates the pricing information
    /// </summary>
    public void UpdatePrice(Money price, string? description = null)
    {
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Description = description?.Trim();
    }

    /// <summary>
    /// Sets the validity period for this pricing rule
    /// </summary>
    public void SetValidityPeriod(DateTime? validFrom, DateTime? validTo)
    {
        ValidateDates(validFrom, validTo);
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// Sets quantity constraints for this pricing rule
    /// </summary>
    public void SetQuantityConstraints(int? minQuantity, int? maxQuantity)
    {
        ValidateQuantities(minQuantity, maxQuantity);
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
    }

    /// <summary>
    /// Sets discount percentage
    /// </summary>
    public void SetDiscount(decimal? discountPercentage)
    {
        ValidateDiscount(discountPercentage);
        DiscountPercentage = discountPercentage;
    }

    /// <summary>
    /// Calculates the effective price after applying discount
    /// </summary>
    public Money GetEffectivePrice()
    {
        if (DiscountPercentage.HasValue && DiscountPercentage.Value > 0)
        {
            var discountAmount = Price.AmountRials * (DiscountPercentage.Value / 100);
            var effectiveAmount = Price.AmountRials - (long)discountAmount;
            return new Money(effectiveAmount);
        }
        return Price;
    }

    /// <summary>
    /// Checks if this pricing is valid for the given date and quantity
    /// </summary>
    public bool IsValidFor(DateTime date, int quantity)
    {
        if (!IsActive)
            return false;

        if (ValidFrom.HasValue && date < ValidFrom.Value)
            return false;

        if (ValidTo.HasValue && date > ValidTo.Value)
            return false;

        if (MinQuantity.HasValue && quantity < MinQuantity.Value)
            return false;

        if (MaxQuantity.HasValue && quantity > MaxQuantity.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Activates the pricing rule
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the pricing rule
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    // Private validation methods
    private static void ValidateQuantities(int? minQuantity, int? maxQuantity)
    {
        if (minQuantity.HasValue && minQuantity.Value < 0)
            throw new ArgumentException("Minimum quantity cannot be negative", nameof(minQuantity));

        if (maxQuantity.HasValue && maxQuantity.Value < 0)
            throw new ArgumentException("Maximum quantity cannot be negative", nameof(maxQuantity));

        if (minQuantity.HasValue && maxQuantity.HasValue && minQuantity.Value > maxQuantity.Value)
            throw new ArgumentException("Minimum quantity cannot be greater than maximum quantity");
    }

    private static void ValidateDiscount(decimal? discountPercentage)
    {
        if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
            throw new ArgumentException("Discount percentage must be between 0 and 100", nameof(discountPercentage));
    }

    private static void ValidateDates(DateTime? validFrom, DateTime? validTo)
    {
        if (validFrom.HasValue && validTo.HasValue && validFrom.Value >= validTo.Value)
            throw new ArgumentException("Valid from date must be before valid to date");
    }
}