using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Domain.Repositories;

/// <summary>
/// Repository interface for FacilityCycleDependency entity
/// </summary>
public interface IFacilityCycleDependencyRepository
{
    /// <summary>
    /// Get dependencies for a cycle
    /// </summary>
    Task<List<FacilityCycleDependency>> GetByCycleIdAsync(Guid cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get cycles that depend on a specific facility
    /// </summary>
    Task<List<FacilityCycleDependency>> GetByRequiredFacilityIdAsync(Guid requiredFacilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if cycle has dependency on facility
    /// </summary>
    Task<bool> HasDependencyAsync(Guid cycleId, Guid requiredFacilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all dependencies for multiple cycles
    /// </summary>
    Task<List<FacilityCycleDependency>> GetByCycleIdsAsync(List<Guid> cycleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add new dependency
    /// </summary>
    Task AddAsync(FacilityCycleDependency dependency, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update dependency
    /// </summary>
    Task UpdateAsync(FacilityCycleDependency dependency, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete dependency
    /// </summary>
    Task DeleteAsync(FacilityCycleDependency dependency, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete all dependencies for a cycle
    /// </summary>
    Task DeleteByCycleIdAsync(Guid cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if there are circular dependencies
    /// </summary>
    Task<bool> HasCircularDependencyAsync(Guid cycleId, Guid requiredFacilityId, CancellationToken cancellationToken = default);
}
