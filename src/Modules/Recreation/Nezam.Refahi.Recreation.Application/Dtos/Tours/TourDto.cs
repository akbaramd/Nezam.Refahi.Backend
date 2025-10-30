using System;
using System.Collections.Generic;
using System.Linq;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Application.Dtos
{
    // ---------------------------------------
    // Domain-specific Summary DTOs (lightweight)
    // ---------------------------------------

    public sealed class TourAgencySummaryDto
    {
        public Guid AgencyId { get; init; }
        public string AgencyName { get; init; } = string.Empty;

        public static TourAgencySummaryDto From(TourAgency a) =>
            new()
            {
                AgencyId = a.AgencyId,
                AgencyName = a.AgencyName?.Trim() ?? string.Empty
            };
    }

    public sealed class TourFeatureSummaryDto
    {
        public Guid FeatureId { get; init; }
        public string Name { get; init; } = string.Empty;

        public static TourFeatureSummaryDto From(TourFeature f) =>
            new()
            {
                FeatureId = f.FeatureId,
                Name = f.Feature?.Name?.Trim() ?? string.Empty
            };
    }

    public sealed class TourPhotoSummaryDto
    {
        public Guid Id { get; init; }
        public string Url { get; init; } = string.Empty;
        public int DisplayOrder { get; init; }

        public static TourPhotoSummaryDto From(TourPhoto p) =>
            new()
            {
                Id = p.Id,
                Url = p.Url ?? string.Empty,
                DisplayOrder = p.DisplayOrder,
            };
    }

    /// <summary>
    /// Minimal, per-participant-type pricing band for list ribbons.
    /// </summary>
    public sealed class TourPricingBandSummaryDto
    {
        public ParticipantType ParticipantType { get; init; }
        public decimal EffectiveMinPriceRials { get; init; }
        public bool HasDiscount { get; init; }

        public static TourPricingBandSummaryDto From(ParticipantType type, decimal minPrice, bool hasDiscount) =>
            new()
            {
                ParticipantType = type,
                EffectiveMinPriceRials = minPrice,
                HasDiscount = hasDiscount
            };
    }

    // ---------------------------------------
    // LIST DTO (lightweight)
    // ---------------------------------------

    /// <summary>
    /// Lightweight Tour DTO for listings. Primitives + compact aggregates + per-entity summary relations.
    /// All photos are listed as summaries; no single “main” is selected by the DTO.
    /// </summary>
    public sealed class TourDto
    {
        // Identity
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;

        // Schedule
        public DateTime TourStart { get; init; }
        public DateTime TourEnd { get; init; }

        // Status
        public bool IsActive { get; init; }
        public TourStatus Status { get; init; }
        public CapacityState CapacityState { get; init; }

        // Registration window (effective across active capacities)
        public DateTime? RegistrationStart { get; init; }
        public DateTime? RegistrationEnd { get; init; }
        public bool IsRegistrationOpen { get; init; }

        // Capacity aggregates (public numbers exclude special capacities)
        public int MaxCapacity { get; init; }
        public int RemainingCapacity { get; init; }
        public int ReservedCapacity { get; init; }
        public double UtilizationPct { get; init; } // 0..100
        public bool IsFullyBooked { get; init; }
        public bool IsNearlyFull { get; init; } // ≥80% utilized

        // Relation summaries (light)
        public List<TourAgencySummaryDto> Agencies { get; init; } = new();
        public List<TourFeatureSummaryDto> Features { get; init; } = new();
        public List<TourPhotoSummaryDto> Photos { get; init; } = new();

        // Pricing summaries (per participant type)
        public decimal? LowestPriceRials { get; init; }
        public decimal? HighestPriceRials { get; init; }
        public bool HasDiscount { get; init; }
        public List<TourPricingBandSummaryDto> PriceBands { get; init; } = new();

        // Current user quick flags (set at query layer)
        public bool CanUserReserve { get; init; }
        public Guid? UserReservationId { get; init; }
        public ReservationStatus? UserReservationStatus { get; init; }

        /// <summary>
        /// Map from domain entity; works with partially loaded graphs.
        /// </summary>
        public static TourDto MapFromEntity(Tour entity, DateTime? nowUtc = null)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var now = nowUtc ?? DateTime.UtcNow;

            // Capacities
            var capacities = entity.Capacities ?? Enumerable.Empty<TourCapacity>();
            var activeCaps = capacities.Where(c => c.IsActive).ToList();

            DateTime? regStart = activeCaps.Count > 0 ? activeCaps.Min(c => c.RegistrationStart) : null;
            DateTime? regEnd   = activeCaps.Count > 0 ? activeCaps.Max(c => c.RegistrationEnd)   : null;
            bool isOpen = activeCaps.Any(c => c.IsRegistrationOpen(now));

            var publicCaps = activeCaps.Where(c => !c.IsSpecial).ToList();
            var maxCap = publicCaps.Sum(c => c.PublicMaxParticipants);
            var remCap = publicCaps.Sum(c => c.PublicRemainingParticipants);
            var used   = Math.Max(0, maxCap - remCap);
            var util   = maxCap > 0 ? (double)used / maxCap * 100d : 0d;
            var full   = maxCap > 0 && remCap <= 0;
            var near   = util >= 80d && !full;

            // Agencies → summaries
            var agencies = (entity.TourAgencies ?? Enumerable.Empty<TourAgency>())
                .Select(TourAgencySummaryDto.From)
                .DistinctBy(x => x.AgencyId)
                .ToList();

            // Features → summaries
            var features = (entity.TourFeatures ?? Enumerable.Empty<TourFeature>())
                .Select(TourFeatureSummaryDto.From)
                .DistinctBy(x => x.FeatureId)
                .ToList();

            // Photos → all as summaries (no “main” field selected)
            var photos = (entity.Photos ?? Enumerable.Empty<TourPhoto>())
                .OrderBy(p => p.DisplayOrder)
                .Select(TourPhotoSummaryDto.From)
                .ToList();

            // Pricing (valid now; fall back to active)
            var pricings = entity.Pricing ?? Enumerable.Empty<TourPricing>();
            var valids = pricings.Where(p =>
                                p.IsActive &&
                                (!p.ValidFrom.HasValue || now >= p.ValidFrom.Value) &&
                                (!p.ValidTo.HasValue   || now <= p.ValidTo.Value))
                                 .ToList();
            if (valids.Count == 0)
                valids = pricings.Where(p => p.IsActive).ToList();

            decimal? low = valids.Count > 0 ? valids.Min(p => p.GetEffectivePrice().AmountRials) : null;
            decimal? high = valids.Count > 0 ? valids.Max(p => p.GetEffectivePrice().AmountRials) : null;
            bool hasDiscount = valids.Any(p => p.DiscountPercentage.HasValue && p.DiscountPercentage.Value > 0);

            var priceBands = valids
                .GroupBy(p => p.ParticipantType)
                .Select(g =>
                {
                    var min = g.Min(x => x.GetEffectivePrice().AmountRials);
                    var anyDiscount = g.Any(x => x.DiscountPercentage.HasValue && x.DiscountPercentage.Value > 0);
                    return TourPricingBandSummaryDto.From(g.Key, min, anyDiscount);
                })
                .OrderBy(x => x.ParticipantType)
                .ToList();

            return new TourDto
            {
                Id = entity.Id,
                Title = entity.Title ?? string.Empty,

                TourStart = entity.TourStart,
                TourEnd   = entity.TourEnd,

                IsActive = entity.IsActive,
                Status = entity.Status,
                CapacityState = ComputeOverallCapacityState(activeCaps),

                RegistrationStart = regStart,
                RegistrationEnd   = regEnd,
                IsRegistrationOpen = isOpen,

                MaxCapacity = maxCap,
                RemainingCapacity = remCap,
                ReservedCapacity = used,
                UtilizationPct = Math.Round(util, 2),
                IsFullyBooked = full,
                IsNearlyFull = near,

                Agencies = agencies,
                Features = features,
                Photos = photos,

                LowestPriceRials  = low,
                HighestPriceRials = high,
                HasDiscount = hasDiscount,
                PriceBands = priceBands,

                // Set by query layer with current user context.
                CanUserReserve = false,
                UserReservationId = null,
                UserReservationStatus = null
            };
        }

        private static CapacityState ComputeOverallCapacityState(IEnumerable<TourCapacity> activeCaps)
        {
            if (activeCaps == null || !activeCaps.Any()) return CapacityState.HasSpare;
            if (activeCaps.Any(c => c.CapacityState == CapacityState.HasSpare)) return CapacityState.HasSpare;
            return activeCaps.Any(c => c.CapacityState == CapacityState.Tight) ? CapacityState.Tight : CapacityState.Full;
        }
    }
}

// ---------------------------------------
// DistinctBy helper (if System.Linq unavailable)
// ---------------------------------------
internal static class LinqDistinctByExt
{
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        var seen = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seen.Add(keySelector(element)))
                yield return element;
        }
    }
}
