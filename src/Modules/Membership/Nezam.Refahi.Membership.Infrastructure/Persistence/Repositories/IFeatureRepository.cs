using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of claim type repository interface using EF Core
/// </summary>
public class FeatureRepository : EfRepository<MembershipDbContext, Features, string>, IFeatureRepository
{
    public FeatureRepository(MembershipDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Features?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .FirstOrDefaultAsync(ct => ct.Id == key, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<Features>> GetActiveClaimTypesAsync(CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .OrderBy(ct => ct.Title)
            .ToListAsync(cancellationToken);
    }



    public async Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(ct => ct.Id == key, cancellationToken:cancellationToken);
    }

    public async Task<bool> ExistsByKeyAsync(string key, string excludeId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(ct => ct.Id == key && ct.Id != excludeId, cancellationToken:cancellationToken);
    }

    protected override IQueryable<Features> PrepareQuery(IQueryable<Features> query)
    {
      return base.PrepareQuery(query);
    }
}