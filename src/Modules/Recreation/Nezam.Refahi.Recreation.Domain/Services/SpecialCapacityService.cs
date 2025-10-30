using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Domain.Services;

/// <summary>
/// Domain service for managing special capacity visibility and access control
/// </summary>
public sealed class SpecialCapacityService
{
    /// <summary>
    /// Filters capacities based on member's special status
    /// Only special members can see special capacities
    /// </summary>
    /// <param name="capacities">List of tour capacities to filter</param>
    /// <param name="memberIsSpecial">Whether the requesting member has special status</param>
    /// <returns>Filtered list of capacities visible to the member</returns>
    public IEnumerable<TourCapacity> FilterCapacitiesForMember(
        IEnumerable<TourCapacity> capacities, 
        bool memberIsSpecial)
    {
        return capacities.Where(capacity => capacity.IsVisibleToMember(memberIsSpecial));
    }

    /// <summary>
    /// Calculates public capacity statistics excluding special capacities
    /// </summary>
    /// <param name="capacities">List of tour capacities</param>
    /// <returns>Public capacity statistics</returns>
    public CapacityStatistics CalculatePublicCapacityStatistics(IEnumerable<TourCapacity> capacities)
    {
        var publicCapacities = capacities.Where(c => !c.IsSpecial).ToList();
        
        var totalMaxParticipants = publicCapacities.Sum(c => c.MaxParticipants);
        var totalAllocatedParticipants = publicCapacities.Sum(c => c.AllocatedParticipants);
        var totalRemainingParticipants = publicCapacities.Sum(c => c.RemainingParticipants);
        var totalUtilizationPercentage = totalMaxParticipants > 0 
            ? (double)totalAllocatedParticipants / totalMaxParticipants * 100 
            : 0;

        return new CapacityStatistics(
            totalMaxParticipants,
            totalAllocatedParticipants,
            totalRemainingParticipants,
            totalUtilizationPercentage,
            publicCapacities.Count
        );
    }

    /// <summary>
    /// Calculates special capacity statistics for VIP members only
    /// </summary>
    /// <param name="capacities">List of tour capacities</param>
    /// <returns>Special capacity statistics</returns>
    public CapacityStatistics CalculateSpecialCapacityStatistics(IEnumerable<TourCapacity> capacities)
    {
        var specialCapacities = capacities.Where(c => c.IsSpecial).ToList();
        
        var totalMaxParticipants = specialCapacities.Sum(c => c.MaxParticipants);
        var totalAllocatedParticipants = specialCapacities.Sum(c => c.AllocatedParticipants);
        var totalRemainingParticipants = specialCapacities.Sum(c => c.RemainingParticipants);
        var totalUtilizationPercentage = totalMaxParticipants > 0 
            ? (double)totalAllocatedParticipants / totalMaxParticipants * 100 
            : 0;

        return new CapacityStatistics(
            totalMaxParticipants,
            totalAllocatedParticipants,
            totalRemainingParticipants,
            totalUtilizationPercentage,
            specialCapacities.Count
        );
    }

    /// <summary>
    /// Validates that a member can access special capacities
    /// </summary>
    /// <param name="memberIsSpecial">Whether the member has special status</param>
    /// <param name="capacityIsSpecial">Whether the capacity is special</param>
    /// <returns>True if access is allowed</returns>
    public bool CanAccessSpecialCapacity(bool memberIsSpecial, bool capacityIsSpecial)
    {
        // Regular capacities are accessible to everyone
        // Special capacities are only accessible to special members
        return !capacityIsSpecial || memberIsSpecial;
    }
}

/// <summary>
/// Value object representing capacity statistics
/// </summary>
public sealed record CapacityStatistics(
    int TotalMaxParticipants,
    int TotalAllocatedParticipants,
    int TotalRemainingParticipants,
    double TotalUtilizationPercentage,
    int CapacityCount
);
