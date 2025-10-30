using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Domain.Repositories;

/// <summary>
/// Repository interface for FacilityRejection entity
/// </summary>
public interface IFacilityRejectionRepository
{
    /// <summary>
    /// Add a new rejection record
    /// </summary>
    Task AddAsync(FacilityRejection rejection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get rejection by ID
    /// </summary>
    Task<FacilityRejection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get rejection by request ID
    /// </summary>
    Task<FacilityRejection?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update rejection record
    /// </summary>
    Task UpdateAsync(FacilityRejection rejection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete rejection record
    /// </summary>
    Task DeleteAsync(FacilityRejection rejection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all rejections for a facility
    /// </summary>
    Task<IEnumerable<FacilityRejection>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get rejections by user ID
    /// </summary>
    Task<IEnumerable<FacilityRejection>> GetByRejectedByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
