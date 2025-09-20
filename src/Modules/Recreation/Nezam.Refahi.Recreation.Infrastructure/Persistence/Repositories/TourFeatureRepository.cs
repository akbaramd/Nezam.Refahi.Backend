using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TourFeature entities
/// </summary>
public class TourFeatureRepository : EfRepository<RecreationDbContext, TourFeature, Guid>, ITourFeatureRepository
{
    public TourFeatureRepository(RecreationDbContext context) : base(context)
    {
    }

    protected override IQueryable<TourFeature> PrepareQuery(IQueryable<TourFeature> query)
    {
        return query;
    }
}