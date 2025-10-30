using MCA.SharedKernel.Domain.Contracts.Repositories;
using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Domain.Repositories;

/// <summary>
/// Repository interface for FacilityCycle aggregate root
/// </summary>
public interface IFacilityCycleRepository : IRepository<FacilityCycle,Guid>
{

    /// <summary>
    /// Get facility cycle with facility details
    /// </summary>
    Task<FacilityCycle?> GetWithFacilityAsync(Guid cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get facility cycle with dependencies
    /// </summary>
    Task<FacilityCycle?> GetWithDependenciesAsync(Guid cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get facility cycle with all related data
    /// </summary>
    Task<FacilityCycle?> GetWithAllDetailsAsync(Guid cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active cycles for a facility
    /// </summary>
    Task<List<FacilityCycle>> GetActiveCyclesForFacilityAsync(Guid facilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get cycles by facility ID
    /// </summary>
    Task<List<FacilityCycle>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get cycles by date range
    /// </summary>
    Task<List<FacilityCycle>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get cycles that are currently accepting applications
    /// </summary>
    Task<List<FacilityCycle>> GetAcceptingApplicationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if member has active request in any cycle of a facility
    /// </summary>
    Task<bool> HasActiveRequestAsync(Guid facilityId, Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get member's completed facilities for dependency checking
    /// </summary>
    Task<List<Guid>> GetMemberCompletedFacilitiesAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get member's active facilities for exclusivity checking
    /// </summary>
    Task<List<Guid>> GetMemberActiveFacilitiesAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add new facility cycle
    /// </summary>
    Task AddAsync(FacilityCycle cycle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update facility cycle
    /// </summary>
    Task UpdateAsync(FacilityCycle cycle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete facility cycle
    /// </summary>
    Task DeleteAsync(FacilityCycle cycle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if cycle name is unique within facility
    /// </summary>
    Task<bool> IsNameUniqueAsync(Guid facilityId, string cycleName, Guid? excludeCycleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get cycles that overlap with given date range
    /// </summary>
    Task<List<FacilityCycle>> GetOverlappingCyclesAsync(Guid facilityId, DateTime startDate, DateTime endDate, Guid? excludeCycleId = null, CancellationToken cancellationToken = default);
}