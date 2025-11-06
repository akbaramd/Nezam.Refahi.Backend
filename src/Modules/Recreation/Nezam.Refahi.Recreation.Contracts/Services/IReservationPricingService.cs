using Nezam.Refahi.Recreation.Contracts.Dtos;

namespace Nezam.Refahi.Recreation.Contracts.Services;

/// <summary>
/// Service interface for determining reservation pricing based on participant type and member capabilities/features
/// </summary>
public interface IReservationPricingService
{
    /// <summary>
    /// Gets the appropriate pricing for a participant in a tour reservation
    /// </summary>
    /// <param name="tourId">The tour ID</param>
    /// <param name="nationalNumber">National number of the participant</param>
    /// <param name="memberCapabilities">Capabilities of the member (if member)</param>
    /// <param name="memberFeatures">Features of the member (if member)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Pricing result containing:
    /// - The selected pricing DTO with all pricing information
    /// - The determined ParticipantType (Member or Guest)
    /// - The effective price
    /// </returns>
    Task<ReservationPricingResult> GetPricingAsync(
        Guid tourId,
        string nationalNumber,
        IEnumerable<string>? memberCapabilities = null,
        IEnumerable<string>? memberFeatures = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pricing for a guest participant (simpler, direct)
    /// </summary>
    /// <param name="tourId">The tour ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pricing result for guest</returns>
    Task<ReservationPricingResult> GetGuestPricingAsync(
        Guid tourId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of pricing determination for a reservation participant
/// </summary>
public class ReservationPricingResult
{
    /// <summary>
    /// The selected pricing information as DTO
    /// </summary>
    public ReservationPricingDto Pricing { get; set; } = null!;

    /// <summary>
    /// The determined participant type (Member or Guest)
    /// </summary>
    public string ParticipantType { get; set; } = string.Empty;

    /// <summary>
    /// The effective price in Rials (after discounts)
    /// </summary>
    public decimal EffectivePriceRials { get; set; }

    /// <summary>
    /// Indicates if this is default pricing (no specific capabilities/features required)
    /// </summary>
    public bool IsDefaultPricing { get; set; }

    /// <summary>
    /// Indicates if pricing has capability or feature requirements
    /// </summary>
    public bool HasRequirements => Pricing.HasRequirements;
}

