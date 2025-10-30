using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourDetailMapper : IMapper<Tour, TourDetailDto>
{
    private readonly IMapper<Tour, TourDto> _tourMapper;
    private readonly IMapper<TourCapacity, CapacityDetailDto> _capacityMapper;
    private readonly IMapper<TourFeature, FeatureDetailDto> _featureMapper;
    private readonly IMapper<TourPhoto, PhotoDetailDto> _photoMapper;
    private readonly IMapper<TourAgency, AgencyDetailDto> _agencyMapper;
    private readonly IMapper<TourRestrictedTour, RestrictedTourSummaryDto> _restrictedTourMapper;
    private readonly IMapper<TourPricing, PricingDetailDto> _pricingMapper;

    public TourDetailMapper(
        IMapper<Tour, TourDto> tourMapper,
        IMapper<TourCapacity, CapacityDetailDto> capacityMapper,
        IMapper<TourFeature, FeatureDetailDto> featureMapper,
        IMapper<TourPhoto, PhotoDetailDto> photoMapper,
        IMapper<TourAgency, AgencyDetailDto> agencyMapper,
        IMapper<TourRestrictedTour, RestrictedTourSummaryDto> restrictedTourMapper,
        IMapper<TourPricing, PricingDetailDto> pricingMapper)
    {
        _tourMapper = tourMapper;
        _capacityMapper = capacityMapper;
        _featureMapper = featureMapper;
        _photoMapper = photoMapper;
        _agencyMapper = agencyMapper;
        _restrictedTourMapper = restrictedTourMapper;
        _pricingMapper = pricingMapper;
    }

    public async Task<TourDetailDto> MapAsync(Tour source, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        
        // Start from base TourDto
        var baseDto = await _tourMapper.MapAsync(source, cancellationToken);

        // Capacities
        var capacities = source.Capacities ?? Enumerable.Empty<TourCapacity>();
        var capacityDtos = await Task.WhenAll(
            capacities.OrderBy(c => c.RegistrationStart)
                .Select(c => _capacityMapper.MapAsync(c, cancellationToken)));

        // Features
        var features = source.TourFeatures ?? Enumerable.Empty<TourFeature>();
        var featureDtos = await Task.WhenAll(features.Select(f => _featureMapper.MapAsync(f, cancellationToken)));

        // Photos
        var photos = (source.Photos ?? Enumerable.Empty<TourPhoto>())
            .OrderBy(p => p.DisplayOrder)
            .ToList();
        var photoDtos = await Task.WhenAll(photos.Select(p => _photoMapper.MapAsync(p, cancellationToken)));

        // Agencies
        var agencies = source.TourAgencies ?? Enumerable.Empty<TourAgency>();
        var agencyDtos = await Task.WhenAll(agencies.Select(a => _agencyMapper.MapAsync(a, cancellationToken)));

        // Restricted Tours
        var restrictedTours = source.TourRestrictedTours ?? Enumerable.Empty<TourRestrictedTour>();
        var restrictedDtos = await Task.WhenAll(restrictedTours.Select(r => _restrictedTourMapper.MapAsync(r, cancellationToken)));

        // Requirements
        var reqCaps = (source.MemberCapabilities ?? Enumerable.Empty<TourMemberCapability>())
            .Select(mc => mc.CapabilityId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var reqFeats = (source.MemberFeatures ?? Enumerable.Empty<TourMemberFeature>())
            .Select(mf => mf.FeatureId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Pricing
        var pricing = source.Pricing ?? Enumerable.Empty<TourPricing>();
        var pricingDtos = await Task.WhenAll(pricing.Select(p => _pricingMapper.MapAsync(p, cancellationToken)));
        var valids = pricingDtos.Where(p =>
            p.IsActive &&
            (!p.ValidFrom.HasValue || now >= p.ValidFrom.Value) &&
            (!p.ValidTo.HasValue || now <= p.ValidTo.Value))
            .ToList();
        if (valids.Count == 0)
            valids = pricingDtos.Where(p => p.IsActive).ToList();

        decimal? low = valids.Count > 0 ? valids.Min(p => p.EffectivePriceRials) : null;
        decimal? high = valids.Count > 0 ? valids.Max(p => p.EffectivePriceRials) : null;
        bool hasDiscount = valids.Any(p => p.DiscountPercentage.HasValue && p.DiscountPercentage.Value > 0);

        // Analytics
        int? total = null, confirmed = null, pending = null, cancelled = null;
        if (source.Reservations != null)
        {
            var res = source.Reservations;
            total = res.Count;
            confirmed = res.Count(r => r.Status == ReservationStatus.Confirmed);
            pending = res.Count(r => r.Status is ReservationStatus.OnHold or ReservationStatus.Draft);
            cancelled = res.Count(r => r.Status == ReservationStatus.Cancelled);
        }

        return new TourDetailDto
        {
            // Base properties
            Id = baseDto.Id,
            Title = baseDto.Title,
            TourStart = baseDto.TourStart,
            TourEnd = baseDto.TourEnd,
            IsActive = baseDto.IsActive,
            Status = baseDto.Status,
            CapacityState = baseDto.CapacityState,
            RegistrationStart = baseDto.RegistrationStart,
            RegistrationEnd = baseDto.RegistrationEnd,
            IsRegistrationOpen = baseDto.IsRegistrationOpen,
            MaxCapacity = baseDto.MaxCapacity,
            RemainingCapacity = baseDto.RemainingCapacity,
            ReservedCapacity = baseDto.ReservedCapacity,
            UtilizationPct = baseDto.UtilizationPct,
            IsFullyBooked = baseDto.IsFullyBooked,
            IsNearlyFull = baseDto.IsNearlyFull,
            LowestPriceRials = low,
            HighestPriceRials = high,
            HasDiscount = hasDiscount,
            Pricing = pricingDtos.ToList(),
            CanUserReserve = baseDto.CanUserReserve,
            UserReservationId = baseDto.UserReservationId,
            UserReservationStatus = baseDto.UserReservationStatus,

            // Detailed-only properties
            Description = source.Description,
            MinAge = source.MinAge,
            MaxAge = source.MaxAge,
            MaxGuestsPerReservation = source.MaxGuestsPerReservation,
            RequiredCapabilities = reqCaps,
            RequiredFeatures = reqFeats,
            Capacities = capacityDtos.ToList(),
            Features = featureDtos.ToList(),
            Photos = photoDtos.ToList(),
            Agencies = agencyDtos.DistinctBy(a => a.AgencyId).ToList(),
            RestrictedTours = restrictedDtos.DistinctBy(r => r.RestrictedTourId).ToList(),
            TotalReservations = total,
            ConfirmedReservations = confirmed,
            PendingReservations = pending,
            CancelledReservations = cancelled,
            UserReservationTrackingCode = null,
            UserReservationDate = null,
            UserReservationExpiryDate = null
        };
    }

    public Task MapAsync(Tour source, TourDetailDto destination, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Update mapping not implemented. Use MapAsync(Tour) instead.");
    }
}

