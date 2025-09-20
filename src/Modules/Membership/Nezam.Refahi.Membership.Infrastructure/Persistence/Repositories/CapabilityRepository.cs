using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

public class CapabilityRepository : EfRepository<MembershipDbContext, Capability, string>, ICapabilityRepository
{
    public CapabilityRepository(MembershipDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Capability?> GetByIdWithClaimTypesAsync(string id, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(c => c.Features)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<Capability>> GetByNameAsync(string namePattern, CancellationToken cancellationToken = default)
    {
        var namePatternLower = namePattern.ToLowerInvariant();
        return await this.PrepareQuery(this._dbSet)
            .Where(c => c.Name.ToLower().Contains(namePatternLower))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Capability>> GetActiveCapabilitiesAsync(CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(c => c.IsActive && c.IsValid())
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Capability>> GetExpiringCapabilitiesAsync(TimeSpan timeThreshold, CancellationToken cancellationToken = default)
    {
        var cutoffTime = DateTimeOffset.UtcNow.Add(timeThreshold);
        return await this.PrepareQuery(this._dbSet)
            .Where(c => c.IsActive &&
                       c.ValidTo.HasValue &&
                       c.ValidTo.Value <= cutoffTime &&
                       !c.IsExpired())
            .OrderBy(c => c.ValidTo)
            .ToListAsync(cancellationToken);
    }




    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(c => c.Name == name, cancellationToken:cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, string excludeId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(c => c.Name == name && c.Id != excludeId, cancellationToken:cancellationToken);
    }

    public async Task<(IEnumerable<Capability> Capabilities, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchTerm = null,
        bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = this.PrepareQuery(this._dbSet);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchTermLower = searchTerm.ToLowerInvariant();
            query = query.Where(c =>
                c.Name.ToLower().Contains(searchTermLower) ||
                c.Description.ToLower().Contains(searchTermLower));
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var capabilities = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (capabilities, totalCount);
    }

    public async Task<int> GetCountByStatusAsync(bool isActive, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .CountAsync(c => c.IsActive == isActive, cancellationToken:cancellationToken);
    }





    protected override IQueryable<Capability> PrepareQuery(IQueryable<Capability> query)
    {
      query = query.Include(x => x.Features);
      return base.PrepareQuery(query);
    }
}