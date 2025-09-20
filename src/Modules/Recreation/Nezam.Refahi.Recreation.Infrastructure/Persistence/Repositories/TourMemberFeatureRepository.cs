using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Infrastructure.Persistence;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TourMemberFeature entity
/// </summary>
public class TourMemberFeatureRepository : EfRepository<RecreationDbContext, TourMemberFeature, Guid>, ITourMemberFeatureRepository
{
    public TourMemberFeatureRepository(RecreationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TourMemberFeature>> GetByTourIdAsync(Guid tourId)
    {
        return await PrepareQuery(_dbSet)
            .Where(tmf => tmf.TourId == tourId)
            .Include(tmf => tmf.Tour)
            .ToListAsync();
    }

    public async Task<IEnumerable<TourMemberFeature>> GetByFeatureIdAsync(string featureId)
    {
        return await PrepareQuery(_dbSet)
            .Where(tmf => tmf.FeatureId == featureId)
            .Include(tmf => tmf.Tour)
            .ToListAsync();
    }

    public async Task<bool> TourRequiresFeatureAsync(Guid tourId, string featureId)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(tmf => tmf.TourId == tourId && tmf.FeatureId == featureId);
    }

    public async Task RemoveAllByTourIdAsync(Guid tourId)
    {
        var features = await PrepareQuery(_dbSet)
            .Where(tmf => tmf.TourId == tourId)
            .ToListAsync();

        if (features.Any())
        {
            _dbSet.RemoveRange(features);
        }
    }

    public async Task RemoveAllByFeatureIdAsync(string featureId)
    {
        var features = await PrepareQuery(_dbSet)
            .Where(tmf => tmf.FeatureId == featureId)
            .ToListAsync();

        if (features.Any())
        {
            _dbSet.RemoveRange(features);
        }
    }

    protected override IQueryable<TourMemberFeature> PrepareQuery(IQueryable<TourMemberFeature> query)
    {
        return query;
    }
}