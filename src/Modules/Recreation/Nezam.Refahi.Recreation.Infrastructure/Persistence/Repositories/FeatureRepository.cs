using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Feature entities
/// </summary>
public class FeatureRepository : EfRepository<RecreationDbContext, Feature, Guid>, IFeatureRepository
{
    public FeatureRepository(RecreationDbContext context) : base(context)
    {
    }

    protected override IQueryable<Feature> PrepareQuery(IQueryable<Feature> query)
    {
        return query;
    }
}