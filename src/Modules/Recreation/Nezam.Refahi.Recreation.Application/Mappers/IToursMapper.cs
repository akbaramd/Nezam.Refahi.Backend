using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourMapper : IMapper<Tour, TourDto>
{
    private readonly IMapper<TourAgency, AgencySummaryDto> _agencyMapper;
    private readonly IMapper<TourFeature, FeatureSummaryDto> _featureMapper;
    private readonly IMapper<TourPhoto, PhotoSummaryDto> _photoMapper;
    private readonly IMapper<TourPricing, PricingDetailDto> _pricingMapper;

    public TourMapper(
        IMapper<TourAgency, AgencySummaryDto> agencyMapper,
        IMapper<TourFeature, FeatureSummaryDto> featureMapper,
        IMapper<TourPhoto, PhotoSummaryDto> photoMapper,
        IMapper<TourPricing, PricingDetailDto> pricingMapper)
    {
        _agencyMapper = agencyMapper;
        _featureMapper = featureMapper;
        _photoMapper = photoMapper;
        _pricingMapper = pricingMapper;
    }

    public async Task<TourDto> MapAsync(Tour source, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Capacities
        var capacities = source.Capacities ?? Enumerable.Empty<TourCapacity>();
        var activeCaps = capacities.Where(c => c.IsActive).ToList();

        DateTime? regStart = activeCaps.Count > 0 ? activeCaps.Min(c => c.RegistrationStart) : null;
        DateTime? regEnd = activeCaps.Count > 0 ? activeCaps.Max(c => c.RegistrationEnd) : null;

        // Use domain behaviors for capacity calculations
        var full = source.IsFullyBooked();
        var near = source.IsNearlyFull();
        
        // Calculate capacity metrics for display
        var publicCaps = activeCaps.Where(c => !c.IsSpecial).ToList();
        var maxCap = publicCaps.Sum(c => c.PublicMaxParticipants);
        var remCap = publicCaps.Sum(c => c.PublicRemainingParticipants);
        var used = Math.Max(0, maxCap - remCap);
        var util = maxCap > 0 ? (double)used / maxCap * 100d : 0d;


        // Agencies → summaries
        var agencies = source.TourAgencies ?? Enumerable.Empty<TourAgency>();
        var agencyDtos = await Task.WhenAll(agencies.Select(a => _agencyMapper.MapAsync(a, cancellationToken)));

        // Features → summaries
        var features = source.TourFeatures ?? Enumerable.Empty<TourFeature>();
        var featureDtos = await Task.WhenAll(features.Select(f => _featureMapper.MapAsync(f, cancellationToken)));

        // Photos → summaries
        var photos = (source.Photos ?? Enumerable.Empty<TourPhoto>())
            .OrderBy(p => p.DisplayOrder)
            .ToList();
        var photoDtos = await Task.WhenAll(photos.Select(p => _photoMapper.MapAsync(p, cancellationToken)));

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

        return new TourDto
        {
            Id = source.Id,
            Title = source.Title ?? string.Empty,
            TourStart = source.TourStart,
            TourEnd = source.TourEnd,
            IsActive = source.IsActive,
            Status = source.Status.ToString(),
            CapacityState = source.GetOverallCapacityState().ToString(),
            RegistrationStart = regStart,
            RegistrationEnd = regEnd,
            IsRegistrationOpen = source.IsRegistrationOpen(now),
            MaxCapacity = maxCap,
            RemainingCapacity = remCap,
            ReservedCapacity = used,
            UtilizationPct = Math.Round(util, 2),
            IsFullyBooked = full,
            IsNearlyFull = near,
            Agencies = agencyDtos.DistinctBy(x => x.AgencyId).ToList(),
            Features = featureDtos.DistinctBy(x => x.FeatureId).ToList(),
            Photos = photoDtos.ToList(),
            LowestPriceRials = low,
            HighestPriceRials = high,
            HasDiscount = hasDiscount,
            Pricing = pricingDtos.ToList(),
        };
    }

    public Task MapAsync(Tour source, TourDto destination, CancellationToken cancellationToken = default)
    {
        // Implementation for update mapping if needed
        throw new NotImplementedException("Update mapping not implemented. Use MapAsync(Tour) instead.");
    }

}