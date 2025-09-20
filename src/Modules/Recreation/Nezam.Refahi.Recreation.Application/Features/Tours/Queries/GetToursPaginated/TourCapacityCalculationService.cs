using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;

/// <summary>
/// Service for calculating tour capacity information efficiently
/// </summary>
public class TourCapacityCalculationService
{
    private readonly ITourReservationRepository _tourReservationRepository;

    public TourCapacityCalculationService(ITourReservationRepository tourReservationRepository)
    {
        _tourReservationRepository = tourReservationRepository;
    }

    /// <summary>
    /// Calculate capacity information for multiple tours in batch for better performance
    /// </summary>
    public async Task<Dictionary<Guid, TourCapacityInfo>> CalculateTourCapacitiesAsync(
        IEnumerable<Tour> tours, 
        CancellationToken cancellationToken = default)
    {
        var tourIds = tours.Select(t => t.Id).ToList();
        var result = new Dictionary<Guid, TourCapacityInfo>();

        // Use batch query for better performance and avoid concurrency issues
        var utilizationDict = await _tourReservationRepository.GetTourUtilizationBatchAsync(tourIds, cancellationToken);

        // Calculate capacity info for each tour
        foreach (var tour in tours)
        {
            var totalUtilization = utilizationDict.GetValueOrDefault(tour.Id, 0);
            var maxCapacity = tour.MaxParticipants;
            var remainingCapacity = Math.Max(0, maxCapacity - totalUtilization);
            var utilizationPercentage = maxCapacity > 0 ? (double)totalUtilization / maxCapacity * 100 : 0;

            result[tour.Id] = new TourCapacityInfo
            {
                TourId = tour.Id,
                MaxCapacity = maxCapacity,
                ReservedCapacity = totalUtilization,
                RemainingCapacity = remainingCapacity,
                UtilizationPercentage = Math.Round(utilizationPercentage, 2),
                IsFullyBooked = remainingCapacity == 0,
                IsNearlyFull = utilizationPercentage > 80
            };
        }

        return result;
    }

    /// <summary>
    /// Calculate capacity information for tour capacities in batch
    /// </summary>
    public async Task<Dictionary<Guid, CapacityInfo>> CalculateCapacitiesAsync(
        IEnumerable<TourCapacity> capacities,
        CancellationToken cancellationToken = default)
    {
        var capacityIds = capacities.Select(c => c.Id).ToList();
        var result = new Dictionary<Guid, CapacityInfo>();

        // Use batch query for better performance and avoid concurrency issues
        var utilizationDict = await _tourReservationRepository.GetCapacityUtilizationBatchAsync(capacityIds, cancellationToken);

        // Calculate capacity info for each capacity
        foreach (var capacity in capacities)
        {
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
                IsNearlyFull = utilizationPercentage > 80
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
    public int MaxCapacity { get; set; }
    public int ReservedCapacity { get; set; }
    public int RemainingCapacity { get; set; }
    public double UtilizationPercentage { get; set; }
    public bool IsFullyBooked { get; set; }
    public bool IsNearlyFull { get; set; }
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
}
