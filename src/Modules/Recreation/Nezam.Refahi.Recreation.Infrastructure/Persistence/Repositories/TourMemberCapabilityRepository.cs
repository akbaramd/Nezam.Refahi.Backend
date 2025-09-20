using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Infrastructure.Persistence;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TourMemberCapability entity
/// </summary>
public class TourMemberCapabilityRepository : EfRepository<RecreationDbContext, TourMemberCapability, Guid>, ITourMemberCapabilityRepository
{
    public TourMemberCapabilityRepository(RecreationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TourMemberCapability>> GetByTourIdAsync(Guid tourId)
    {
        return await PrepareQuery(_dbSet)
            .Where(tmc => tmc.TourId == tourId)
            .Include(tmc => tmc.Tour)
            .ToListAsync();
    }

    public async Task<IEnumerable<TourMemberCapability>> GetByCapabilityIdAsync(string capabilityId)
    {
        return await PrepareQuery(_dbSet)
            .Where(tmc => tmc.CapabilityId == capabilityId)
            .Include(tmc => tmc.Tour)
            .ToListAsync();
    }

    public async Task<bool> TourRequiresCapabilityAsync(Guid tourId, string capabilityId)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(tmc => tmc.TourId == tourId && tmc.CapabilityId == capabilityId);
    }

    public async Task RemoveAllByTourIdAsync(Guid tourId)
    {
        var capabilities = await PrepareQuery(_dbSet)
            .Where(tmc => tmc.TourId == tourId)
            .ToListAsync();

        if (capabilities.Any())
        {
            _dbSet.RemoveRange(capabilities);
        }
    }

    public async Task RemoveAllByCapabilityIdAsync(string capabilityId)
    {
        var capabilities = await PrepareQuery(_dbSet)
            .Where(tmc => tmc.CapabilityId == capabilityId)
            .ToListAsync();

        if (capabilities.Any())
        {
            _dbSet.RemoveRange(capabilities);
        }
    }

    protected override IQueryable<TourMemberCapability> PrepareQuery(IQueryable<TourMemberCapability> query)
    {
        return query;
    }
}