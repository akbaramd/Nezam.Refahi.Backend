using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for FeatureCategory entities
/// </summary>
public class FeatureCategoryRepository : EfRepository<RecreationDbContext, FeatureCategory, Guid>, IFeatureCategoryRepository
{
    public FeatureCategoryRepository(RecreationDbContext context) : base(context)
    {
    }

    protected override IQueryable<FeatureCategory> PrepareQuery(IQueryable<FeatureCategory> query)
    {
        return query;
    }
}