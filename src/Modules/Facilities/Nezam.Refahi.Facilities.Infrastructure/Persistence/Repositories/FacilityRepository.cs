using Microsoft.EntityFrameworkCore;
using MCA.SharedKernel.Infrastructure.Repositories;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Infrastructure.Persistence;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Facility entity
/// </summary>
public class FacilityRepository : EfRepository<FacilitiesDbContext, Facility, Guid>, IFacilityRepository
{
    public FacilityRepository(FacilitiesDbContext context) : base(context)
    {
    }

    protected override IQueryable<Facility> PrepareQuery(IQueryable<Facility> query)
    {
        // Include Cycles navigation property for querying purposes
        // This is needed for specifications that filter by cycle status (e.g., FacilityPaginatedSpec)
        // and for mappers that need cycle statistics (e.g., FacilityCycleStatisticsMapper)
        // Also include Requests for each cycle to calculate UsedQuota correctly
        return query
            .Include(f => f.Cycles)
                .ThenInclude(c => c.Requests);
    }

    public async Task<Facility?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(f => f.Code == code, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Facility>> GetActiveFacilitiesAsync(CancellationToken cancellationToken = default)
    {
        // Note: Cycles are now separate Aggregate Roots
        // This method should use FacilityCycleRepository to find active cycles first,
        // then get the facilities. For now, returning all facilities.
        // This should be handled at Application Service layer using both repositories.
        return await FindAsync(f => true, cancellationToken: cancellationToken);
    }

    public async Task<Facility?> GetWithFeaturesAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(f => f.Id == facilityId, cancellationToken: cancellationToken);
    }

    public async Task<Facility?> GetWithCapabilityPoliciesAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(f => f.Id == facilityId, cancellationToken: cancellationToken);
    }

    public async Task<Facility?> GetWithCyclesAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        // Cycles are included via PrepareQuery, so this works correctly
        return await FindOneAsync(f => f.Id == facilityId, cancellationToken: cancellationToken);
    }

    public async Task<Facility?> GetWithAllDataAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        // Cycles are included via PrepareQuery, so this works correctly
        return await FindOneAsync(f => f.Id == facilityId, cancellationToken: cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(f => f.Code == code, cancellationToken: cancellationToken);
    }



    public async Task<IEnumerable<Facility>> GetFacilitiesAsync(FacilityQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        // Note: OnlyActive filter requires FacilityCycleRepository
        // This should be handled at Application Service layer using both repositories
        // For now, we skip this filter

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(f => f.Name.ToLower().Contains(searchTerm) || 
                                    f.Code.ToLower().Contains(searchTerm) ||
                                    (f.Description != null && f.Description.ToLower().Contains(searchTerm)));
        }

        // Apply pagination
        var skip = (parameters.Page - 1) * parameters.PageSize;
        return await query
            .OrderBy(f => f.Name)
            .Skip(skip)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetFacilitiesCountAsync(FacilityQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        // Note: OnlyActive filter requires FacilityCycleRepository
        // This should be handled at Application Service layer using both repositories
        // For now, we skip this filter

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(f => f.Name.ToLower().Contains(searchTerm) || 
                                    f.Code.ToLower().Contains(searchTerm) ||
                                    (f.Description != null && f.Description.ToLower().Contains(searchTerm)));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<Facility?> GetByIdWithDetailsAsync(Guid facilityId, bool includeCycles = true, bool includeFeatures = true, bool includePolicies = true, CancellationToken cancellationToken = default)
    {
        // Cycles are included via PrepareQuery, so this works correctly
        // The parameters are kept for API compatibility but don't affect the query
        return await FindOneAsync(f => f.Id == facilityId, cancellationToken: cancellationToken);
    }
}
