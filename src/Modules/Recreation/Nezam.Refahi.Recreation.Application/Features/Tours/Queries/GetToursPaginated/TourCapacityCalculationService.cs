using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Domain.Services;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;

/// <summary>
/// Service for calculating tour capacity information efficiently
/// </summary>
public class TourCapacityCalculationService
{
    private readonly ITourReservationRepository _tourReservationRepository;
    private readonly SpecialCapacityService _specialCapacityService;

    public TourCapacityCalculationService(ITourReservationRepository tourReservationRepository)
    {
        _tourReservationRepository = tourReservationRepository;
        _specialCapacityService = new SpecialCapacityService();
    }

    /// <summary>
    /// Calculate capacity information for multiple tours in batch for better performance
    /// </summary>
    public async Task<Dictionary<Guid, TourCapacityInfo>> CalculateTourCapacitiesAsync(
        IEnumerable<Tour> tours, 
        bool memberIsSpecial = false,
        CancellationToken cancellationToken = default)
    {
        var tourIds = tours.Select(t => t.Id).ToList();
        var result = new Dictionary<Guid, TourCapacityInfo>();

        // Use batch query for better performance and avoid concurrency issues
        var utilizationDict = await _tourReservationRepository.GetTourUtilizationBatchAsync(tourIds, cancellationToken);

        // Calculate capacity info for each tour
        foreach (var tour in tours)
        {
            // Get all capacities for this tour
            var allCapacities = tour.GetActiveCapacities().ToList();
            
            // Filter capacities based on member's special status
            var visibleCapacities = _specialCapacityService.FilterCapacitiesForMember(allCapacities, memberIsSpecial).ToList();
            
            // Calculate public capacity statistics (excluding special capacities)
            var publicStats = _specialCapacityService.CalculatePublicCapacityStatistics(allCapacities);
            
            // Calculate special capacity statistics (only for special members)
            var specialStats = memberIsSpecial 
                ? _specialCapacityService.CalculateSpecialCapacityStatistics(allCapacities)
                : new CapacityStatistics(0, 0, 0, 0, 0);

            result[tour.Id] = new TourCapacityInfo
            {
                TourId = tour.Id,
                MaxCapacity = publicStats.TotalMaxParticipants,
                ReservedCapacity = publicStats.TotalAllocatedParticipants,
                RemainingCapacity = publicStats.TotalRemainingParticipants,
                UtilizationPercentage = Math.Round(publicStats.TotalUtilizationPercentage, 2),
                IsFullyBooked = publicStats.TotalRemainingParticipants == 0,
                IsNearlyFull = publicStats.TotalUtilizationPercentage > 80,
                
                // Special capacity info (only for special members)
                SpecialMaxCapacity = specialStats.TotalMaxParticipants,
                SpecialReservedCapacity = specialStats.TotalAllocatedParticipants,
                SpecialRemainingCapacity = specialStats.TotalRemainingParticipants,
                SpecialUtilizationPercentage = Math.Round(specialStats.TotalUtilizationPercentage, 2),
                HasSpecialCapacities = specialStats.CapacityCount > 0
            };
        }

        return result;
    }

    /// <summary>
    /// Calculate capacity information for tour capacities in batch
    /// </summary>
    public async Task<Dictionary<Guid, CapacityInfo>> CalculateCapacitiesAsync(
        IEnumerable<TourCapacity> capacities,
        bool memberIsSpecial = false,
        CancellationToken cancellationToken = default)
    {
        var capacityIds = capacities.Select(c => c.Id).ToList();
        var result = new Dictionary<Guid, CapacityInfo>();

        // Use batch query for better performance and avoid concurrency issues
        var utilizationDict = await _tourReservationRepository.GetCapacityUtilizationBatchAsync(capacityIds, cancellationToken);

        // Calculate capacity info for each capacity
        foreach (var capacity in capacities)
        {
            // Check if this capacity is visible to the member
            if (!capacity.IsVisibleToMember(memberIsSpecial))
                continue; // Skip special capacities for non-special members

            var utilization = utilizationDict.GetValueOrDefault(capacity.Id, 0);
            var remainingParticipants = Math.Max(0, capacity.MaxParticipants - utilization);
            var utilizationPercentage = capacity.MaxParticipants > 0 ? (double)utilization / capacity.MaxParticipants * 100 : 0;

            result[capacity.Id] = new CapacityInfo
            {
                CapacityId = capacity.Id,
                MaxParticipants = capacity.MaxParticipants,
                AllocatedParticipants = utilization,
                RemainingParticipants = remainingParticipants,
                UtilizationPercentage = Math.Round(utilizationPercentage, 2),
                IsFullyBooked = remainingParticipants == 0,
                IsNearlyFull = utilizationPercentage > 80,
                IsSpecial = capacity.IsSpecial
            };
        }

        return result;
    }
}

/// <summary>
/// Tour capacity information
/// </summary>
public class TourCapacityInfo
{
    public Guid TourId { get; set; }
    
    // Public capacity info (excluding special capacities)
    public int MaxCapacity { get; set; }
    public int ReservedCapacity { get; set; }
    public int RemainingCapacity { get; set; }
    public double UtilizationPercentage { get; set; }
    public bool IsFullyBooked { get; set; }
    public bool IsNearlyFull { get; set; }
    
    // Special capacity info (only for special members)
    public int SpecialMaxCapacity { get; set; }
    public int SpecialReservedCapacity { get; set; }
    public int SpecialRemainingCapacity { get; set; }
    public double SpecialUtilizationPercentage { get; set; }
    public bool HasSpecialCapacities { get; set; }
}

/// <summary>
/// Individual capacity information
/// </summary>
public class CapacityInfo
{
    public Guid CapacityId { get; set; }
    public int MaxParticipants { get; set; }
    public int AllocatedParticipants { get; set; }
    public int RemainingParticipants { get; set; }
    public double UtilizationPercentage { get; set; }
    public bool IsFullyBooked { get; set; }
    public bool IsNearlyFull { get; set; }
    public bool IsSpecial { get; set; }
}
