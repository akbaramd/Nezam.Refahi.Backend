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
        return query
            .Include(f => f.Features)
            .Include(f => f.CapabilityPolicies)
            .Include(f => f.Cycles)
                .ThenInclude(c => c.Dependencies);
    }

    public async Task<Facility?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(f => f.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Facility>> GetByTypeAsync(FacilityType type, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(f => f.Type == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Facility>> GetByStatusAsync(FacilityStatus status, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(f => f.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Facility>> GetActiveFacilitiesAsync(CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(f => f.Status == FacilityStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<Facility?> GetWithFeaturesAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(f => f.Id == facilityId, cancellationToken);
    }

    public async Task<Facility?> GetWithCapabilityPoliciesAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(f => f.Id == facilityId, cancellationToken);
    }

    public async Task<Facility?> GetWithCyclesAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(f => f.Id == facilityId, cancellationToken);
    }

    public async Task<Facility?> GetWithAllDataAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(f => f.Id == facilityId, cancellationToken);
    }

    public async Task<IEnumerable<Facility>> GetByExclusiveSetAsync(string exclusiveSetId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(f => f.Cycles.Any(c => c.ExclusiveSetId == exclusiveSetId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(f => f.Code == code, cancellationToken);
    }

    public async Task<Dictionary<FacilityStatus, int>> GetStatusStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var statistics = await PrepareQuery(_dbSet)
            .GroupBy(f => f.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return statistics.ToDictionary(s => s.Status, s => s.Count);
    }

    public async Task<IEnumerable<Facility>> GetFacilitiesAsync(FacilityQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        // Apply filters
        if (!string.IsNullOrEmpty(parameters.Type) && Enum.TryParse<FacilityType>(parameters.Type, out var type))
        {
            query = query.Where(f => f.Type == type);
        }

        if (!string.IsNullOrEmpty(parameters.Status) && Enum.TryParse<FacilityStatus>(parameters.Status, out var status))
        {
            query = query.Where(f => f.Status == status);
        }

        if (parameters.OnlyActive)
        {
            query = query.Where(f => f.Status == FacilityStatus.Active);
        }

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

        // Apply filters
        if (!string.IsNullOrEmpty(parameters.Type) && Enum.TryParse<FacilityType>(parameters.Type, out var type))
        {
            query = query.Where(f => f.Type == type);
        }

        if (!string.IsNullOrEmpty(parameters.Status) && Enum.TryParse<FacilityStatus>(parameters.Status, out var status))
        {
            query = query.Where(f => f.Status == status);
        }

        if (parameters.OnlyActive)
        {
            query = query.Where(f => f.Status == FacilityStatus.Active);
        }

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
        // Since PrepareQuery already includes all relationships, we can use it directly
        // The parameters are kept for API compatibility but don't affect the query
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(f => f.Id == facilityId, cancellationToken);
    }
}
