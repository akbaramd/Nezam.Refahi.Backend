using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Domain.Repositories;

/// <summary>
/// Repository interface for facility entities
/// </summary>
public interface IFacilityRepository : IRepository<Facility, Guid>
{
    /// <summary>
    /// Gets facility by code
    /// </summary>
    Task<Facility?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

  
    /// <summary>
    /// Gets active facilities
    /// </summary>
    Task<IEnumerable<Facility>> GetActiveFacilitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets facility with features
    /// </summary>
    Task<Facility?> GetWithFeaturesAsync(Guid facilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets facility with capability policies
    /// </summary>
    Task<Facility?> GetWithCapabilityPoliciesAsync(Guid facilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets facility with cycles
    /// </summary>
    Task<Facility?> GetWithCyclesAsync(Guid facilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets facility with all related data
    /// </summary>
    Task<Facility?> GetWithAllDataAsync(Guid facilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if facility code exists
    /// </summary>
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);

  
    /// <summary>
    /// Gets facilities with pagination and filtering
    /// </summary>
    Task<IEnumerable<Facility>> GetFacilitiesAsync(FacilityQueryParameters parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets facilities count with filtering
    /// </summary>
    Task<int> GetFacilitiesCountAsync(FacilityQueryParameters parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets facility by ID with optional related data
    /// </summary>
    Task<Facility?> GetByIdWithDetailsAsync(Guid facilityId, bool includeCycles = true, bool includeFeatures = true, bool includePolicies = true, CancellationToken cancellationToken = default);
}

/// <summary>
/// Query parameters for facility search
/// </summary>
public class FacilityQueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool OnlyActive { get; set; } = true;
}
