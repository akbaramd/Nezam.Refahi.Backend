using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Contracts.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Services;

/// <summary>
/// Service for determining reservation pricing based on participant type and member capabilities/features
/// 
/// Pricing Logic:
/// 1. If participant is Guest → Return Guest pricing (default)
/// 2. If participant is Member:
///    a. Check if there's pricing that matches member's capabilities/features
///    b. If matching pricing found → Return that pricing
///    c. If no matching pricing → Return default Member pricing
/// </summary>
public class ReservationPricingService : IReservationPricingService
{
    private readonly ITourRepository _tourRepository;
    private readonly IMemberService _memberService;
    private readonly ILogger<ReservationPricingService> _logger;

    public ReservationPricingService(
        ITourRepository tourRepository,
        IMemberService memberService,
        ILogger<ReservationPricingService> logger)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ReservationPricingResult> GetPricingAsync(
        Guid tourId,
        string nationalNumber,
        IEnumerable<string>? memberCapabilities = null,
        IEnumerable<string>? memberFeatures = null,
        CancellationToken cancellationToken = default)
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID is required", nameof(tourId));
        if (string.IsNullOrWhiteSpace(nationalNumber))
            throw new ArgumentException("National number is required", nameof(nationalNumber));

        // Load tour from repository
        var tour = await _tourRepository.FindOneAsync(t => t.Id == tourId, cancellationToken);
        if (tour == null)
        {
            _logger.LogError("Tour {TourId} not found", tourId);
            throw new InvalidOperationException($"تور با شناسه {tourId} یافت نشد");
        }

        var nationalId = new NationalId(nationalNumber);

        // Step 1: Check if person is a member
        var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
        
        if (memberDetail == null)
        {
            // Not a member → Guest pricing
            _logger.LogInformation(
                "Participant with national number {NationalNumber} is not a member - using Guest pricing",
                nationalNumber);
            return await GetGuestPricingAsync(tourId, cancellationToken);
        }

        // Step 2: Check if member has active membership
        var hasActiveMembership = await _memberService.HasActiveMembershipAsync(nationalId);
        if (!hasActiveMembership)
        {
            // Member exists but membership is inactive → Guest pricing
            _logger.LogInformation(
                "Participant with national number {NationalNumber} is a member but membership is inactive - using Guest pricing",
                nationalNumber);
            return await GetGuestPricingAsync(tourId, cancellationToken);
        }

        // Step 3: Member with active membership → Determine Member pricing
        // Use provided capabilities/features or fetch from memberDetail
        var capabilities = memberCapabilities?.ToList() ?? memberDetail.Capabilities?.ToList() ?? new List<string>();
        var features = memberFeatures?.ToList() ?? memberDetail.Features?.ToList() ?? new List<string>();

        _logger.LogInformation(
            "Participant with national number {NationalNumber} is a member with {CapabilityCount} capabilities and {FeatureCount} features - determining Member pricing",
            nationalNumber, capabilities.Count, features.Count);

        // Get pricing using Tour's GetPricing method which handles:
        // - Priority 1: Pricing with matching capabilities/features
        // - Priority 2: Default Member pricing
        // - Priority 3: Any Member pricing without requirements
        var memberPricing = tour.GetPricing(
            ParticipantType.Member,
            date: DateTime.UtcNow,
            memberCapabilities: capabilities.Any() ? capabilities : null,
            memberFeatures: features.Any() ? features : null);

        if (memberPricing == null)
        {
            _logger.LogWarning(
                "No Member pricing found for tour {TourId} - falling back to Guest pricing",
                tourId);
            return await GetGuestPricingAsync(tourId, cancellationToken);
        }

        var effectivePrice = memberPricing.GetEffectivePrice();
        var isDefault = memberPricing.IsDefault && !memberPricing.HasRequirements();
        var discountAmount = memberPricing.DiscountPercentage.HasValue && memberPricing.DiscountPercentage.Value > 0
            ? memberPricing.BasePrice.AmountRials - effectivePrice.AmountRials
            : (decimal?)null;

        _logger.LogInformation(
            "Selected Member pricing {PricingId} for tour {TourId}, participant {NationalNumber}. " +
            "IsDefault: {IsDefault}, HasRequirements: {HasRequirements}, EffectivePrice: {EffectivePrice}",
            memberPricing.Id, tourId, nationalNumber, isDefault, memberPricing.HasRequirements(), effectivePrice.AmountRials);

        // Map TourPricing entity to DTO
        var pricingDto = new ReservationPricingDto
        {
            PricingId = memberPricing.Id,
            TourId = tourId,
            ParticipantType = ParticipantType.Member.ToString(),
            BasePriceRials = memberPricing.BasePrice.AmountRials,
            EffectivePriceRials = effectivePrice.AmountRials,
            DiscountPercentage = memberPricing.DiscountPercentage,
            DiscountAmountRials = discountAmount,
            ValidFrom = memberPricing.ValidFrom,
            ValidTo = memberPricing.ValidTo,
            IsActive = memberPricing.IsActive,
            IsDefault = memberPricing.IsDefault,
            HasRequirements = memberPricing.HasRequirements(),
            RequiredCapabilityIds = memberPricing.Capabilities.Select(c => c.CapabilityId).ToList(),
            RequiredFeatureIds = memberPricing.Features.Select(f => f.FeatureId).ToList(),
            IsEarlyBird = memberPricing.IsEarlyBird,
            IsLastMinute = memberPricing.IsLastMinute,
            Description = memberPricing.Description
        };

        return new ReservationPricingResult
        {
            Pricing = pricingDto,
            ParticipantType = ParticipantType.Member.ToString(),
            EffectivePriceRials = effectivePrice.AmountRials,
            IsDefaultPricing = isDefault
        };
    }

    public async Task<ReservationPricingResult> GetGuestPricingAsync(
        Guid tourId,
        CancellationToken cancellationToken = default)
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID is required", nameof(tourId));

        // Load tour from repository
        var tour = await _tourRepository.FindOneAsync(t => t.Id == tourId, cancellationToken);
        if (tour == null)
        {
            _logger.LogError("Tour {TourId} not found", tourId);
            throw new InvalidOperationException($"تور با شناسه {tourId} یافت نشد");
        }

        // Get Guest pricing (default or first available)
        var guestPricing = tour.GetPricing(ParticipantType.Guest, date: DateTime.UtcNow);

        if (guestPricing == null)
        {
            _logger.LogError("No Guest pricing found for tour {TourId}", tourId);
            throw new InvalidOperationException($"قیمت‌گذاری برای مهمان در تور {tour.Title} یافت نشد");
        }

        var effectivePrice = guestPricing.GetEffectivePrice();
        var isDefault = guestPricing.IsDefault && !guestPricing.HasRequirements();
        var discountAmount = guestPricing.DiscountPercentage.HasValue && guestPricing.DiscountPercentage.Value > 0
            ? guestPricing.BasePrice.AmountRials - effectivePrice.AmountRials
            : (decimal?)null;

        _logger.LogInformation(
            "Selected Guest pricing {PricingId} for tour {TourId}. " +
            "IsDefault: {IsDefault}, HasRequirements: {HasRequirements}, EffectivePrice: {EffectivePrice}",
            guestPricing.Id, tourId, isDefault, guestPricing.HasRequirements(), effectivePrice.AmountRials);

        // Map TourPricing entity to DTO
        var pricingDto = new ReservationPricingDto
        {
            PricingId = guestPricing.Id,
            TourId = tourId,
            ParticipantType = ParticipantType.Guest.ToString(),
            BasePriceRials = guestPricing.BasePrice.AmountRials,
            EffectivePriceRials = effectivePrice.AmountRials,
            DiscountPercentage = guestPricing.DiscountPercentage,
            DiscountAmountRials = discountAmount,
            ValidFrom = guestPricing.ValidFrom,
            ValidTo = guestPricing.ValidTo,
            IsActive = guestPricing.IsActive,
            IsDefault = guestPricing.IsDefault,
            HasRequirements = guestPricing.HasRequirements(),
            RequiredCapabilityIds = guestPricing.Capabilities.Select(c => c.CapabilityId).ToList(),
            RequiredFeatureIds = guestPricing.Features.Select(f => f.FeatureId).ToList(),
            IsEarlyBird = guestPricing.IsEarlyBird,
            IsLastMinute = guestPricing.IsLastMinute,
            Description = guestPricing.Description
        };

        return new ReservationPricingResult
        {
            Pricing = pricingDto,
            ParticipantType = ParticipantType.Guest.ToString(),
            EffectivePriceRials = effectivePrice.AmountRials,
            IsDefaultPricing = isDefault
        };
    }
}

