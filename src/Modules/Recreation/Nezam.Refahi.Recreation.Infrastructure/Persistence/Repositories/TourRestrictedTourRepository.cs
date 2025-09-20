using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Infrastructure.Persistence;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TourRestrictedTour entity
/// </summary>
public class TourRestrictedTourRepository : EfRepository<RecreationDbContext, TourRestrictedTour, Guid>, ITourRestrictedTourRepository
{
    public TourRestrictedTourRepository(RecreationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TourRestrictedTour>> GetRestrictedByTourIdAsync(Guid tourId)
    {
        return await PrepareQuery(_dbSet)
            .Where(trt => trt.TourId == tourId)
            .Include(trt => trt.Tour)
            .Include(trt => trt.RestrictedTour)
            .ToListAsync();
    }

    public async Task<IEnumerable<TourRestrictedTour>> GetRestrictingTourIdAsync(Guid restrictedTourId)
    {
        return await PrepareQuery(_dbSet)
            .Where(trt => trt.RestrictedTourId == restrictedTourId)
            .Include(trt => trt.Tour)
            .Include(trt => trt.RestrictedTour)
            .ToListAsync();
    }

    public async Task<bool> TourRestrictsOtherAsync(Guid tourId, Guid restrictedTourId)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(trt => trt.TourId == tourId && trt.RestrictedTourId == restrictedTourId);
    }

    public async Task RemoveAllByTourIdAsync(Guid tourId)
    {
        var restrictions = await PrepareQuery(_dbSet)
            .Where(trt => trt.TourId == tourId || trt.RestrictedTourId == tourId)
            .ToListAsync();

        if (restrictions.Any())
        {
            _dbSet.RemoveRange(restrictions);
        }
    }

    public async Task<IEnumerable<TourRestrictedTour>> GetMutualRestrictionsAsync(Guid tourId1, Guid tourId2)
    {
        return await PrepareQuery(_dbSet)
            .Where(trt =>
                (trt.TourId == tourId1 && trt.RestrictedTourId == tourId2) ||
                (trt.TourId == tourId2 && trt.RestrictedTourId == tourId1))
            .Include(trt => trt.Tour)
            .Include(trt => trt.RestrictedTour)
            .ToListAsync();
    }

    protected override IQueryable<TourRestrictedTour> PrepareQuery(IQueryable<TourRestrictedTour> query)
    {
        return query;
    }
}