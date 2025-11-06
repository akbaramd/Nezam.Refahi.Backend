using Microsoft.EntityFrameworkCore;
using MCA.SharedKernel.Infrastructure.Repositories;
using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Infrastructure.Persistence;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IFacilityCycleRepository
/// </summary>
public class FacilityCycleRepository : EfRepository<FacilitiesDbContext, FacilityCycle, Guid>, IFacilityCycleRepository
{
    public FacilityCycleRepository(FacilitiesDbContext context) : base(context)
    {
    }

    protected override IQueryable<FacilityCycle> PrepareQuery(IQueryable<FacilityCycle> query)
    {
        // Include Requests navigation property for querying purposes
        // This is needed for mappers that calculate cycle statistics (e.g., CycleStatisticsMapper)
        return query
            .Include(c => c.Facility)
            .Include(c => c.Dependencies)
            .Include(c => c.PriceOptions)
            .Include(c => c.Features)
            .Include(c => c.Capabilities)
            .Include(c => c.Requests);
    }

    public async Task<FacilityCycle?> GetWithFacilityAsync(Guid cycleId, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(c => c.Id == cycleId, cancellationToken: cancellationToken);
    }

    public async Task<FacilityCycle?> GetWithDependenciesAsync(Guid cycleId, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(c => c.Id == cycleId, cancellationToken: cancellationToken);
    }

    public async Task<FacilityCycle?> GetWithAllDetailsAsync(Guid cycleId, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(c => c.Id == cycleId, cancellationToken: cancellationToken);
    }

    public async Task<List<FacilityCycle>> GetActiveCyclesForFacilityAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        var cycles = (await FindAsync(c => c.FacilityId == facilityId && c.Status == Domain.Enums.FacilityCycleStatus.Active, cancellationToken: cancellationToken)).ToList();
        return cycles.OrderBy(c => c.StartDate).ToList();
    }

    public async Task<List<FacilityCycle>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        var cycles = (await FindAsync(c => c.FacilityId == facilityId, cancellationToken: cancellationToken)).ToList();
        return cycles.OrderByDescending(c => c.StartDate).ToList();
    }

    public async Task<List<FacilityCycle>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var cycles = (await FindAsync(c => c.StartDate <= endDate && c.EndDate >= startDate, cancellationToken: cancellationToken)).ToList();
        return cycles.OrderBy(c => c.StartDate).ToList();
    }

    public async Task<List<FacilityCycle>> GetAcceptingApplicationsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        // Note: UsedQuota is computed from Requests.Count, so we need to filter in memory
        // or use a more complex query. For now, we'll filter active cycles and check quota after loading
        var allActiveCycles = (await FindAsync(c => c.Status == Domain.Enums.FacilityCycleStatus.Active &&
                                                    c.StartDate <= now && c.EndDate >= now, 
                                                    cancellationToken: cancellationToken)).ToList();
        var cycles = allActiveCycles.Where(c => c.UsedQuota < c.Quota).ToList();
        return cycles.OrderBy(c => c.StartDate).ToList();
    }

    public async Task<bool> HasActiveRequestAsync(Guid facilityId, Guid memberId, CancellationToken cancellationToken = default)
    {
        // This method needs to access FacilityRequests table which is in a different repository
        // This should be handled at the application/service layer using both repositories
        // For now, return false as a placeholder
        await Task.CompletedTask;
        return false;
    }

    public async Task<List<Guid>> GetMemberCompletedFacilitiesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        // This method needs to access FacilityRequests table which is in a different repository
        // This should be handled at the application/service layer using both repositories
        // For now, return empty list as a placeholder
        await Task.CompletedTask;
        return new List<Guid>();
    }

    public async Task<List<Guid>> GetMemberActiveFacilitiesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        // This method needs to access FacilityRequests table which is in a different repository
        // This should be handled at the application/service layer using both repositories
        // For now, return empty list as a placeholder
        await Task.CompletedTask;
        return new List<Guid>();
    }

    public async Task<bool> IsNameUniqueAsync(Guid facilityId, string cycleName, Guid? excludeCycleId = null, CancellationToken cancellationToken = default)
    {
        if (excludeCycleId.HasValue)
        {
            return !await ExistsAsync(c => c.FacilityId == facilityId && 
                                         c.Name == cycleName && 
                                         c.Id != excludeCycleId.Value, 
                                         cancellationToken: cancellationToken);
        }

        return !await ExistsAsync(c => c.FacilityId == facilityId && c.Name == cycleName, cancellationToken: cancellationToken);
    }

    public async Task<List<FacilityCycle>> GetOverlappingCyclesAsync(Guid facilityId, DateTime startDate, DateTime endDate, Guid? excludeCycleId = null, CancellationToken cancellationToken = default)
    {
        if (excludeCycleId.HasValue)
        {
            var cycles = (await FindAsync(c => c.FacilityId == facilityId &&
                                              c.StartDate < endDate && 
                                              c.EndDate > startDate &&
                                              c.Id != excludeCycleId.Value, 
                                              cancellationToken: cancellationToken)).ToList();
            return cycles;
        }

        var allCycles = (await FindAsync(c => c.FacilityId == facilityId &&
                                            c.StartDate < endDate && 
                                            c.EndDate > startDate, 
                                            cancellationToken: cancellationToken)).ToList();
        return allCycles;
    }
}