using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Infrastructure.Persistence;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IMemberFeatureRepository
/// </summary>
public class MemberFeatureRepository : EfRepository<MembershipDbContext, MemberFeature, Guid>, IMemberFeatureRepository
{
    public MemberFeatureRepository(MembershipDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MemberFeature>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(mf => mf.MemberId == memberId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetValidFeaturesByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        
        return await PrepareQuery(_dbSet)
            .Where(mf => mf.MemberId == memberId &&
                        mf.IsActive &&
                        (mf.ValidFrom == null || mf.ValidFrom <= now) &&
                        (mf.ValidTo == null || mf.ValidTo > now))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetByFeatureKeyAsync(string featureKey, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(mf => mf.FeatureKey == featureKey)
            .OrderByDescending(mf => mf.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetByFeatureKeysAsync(IEnumerable<string> featureKeys, CancellationToken cancellationToken = default)
    {
        var featureKeyList = featureKeys.ToList();
        
        return await PrepareQuery(_dbSet)
            .Where(mf => featureKeyList.Contains(mf.FeatureKey))
            .OrderByDescending(mf => mf.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetByMemberIdsAsync(IEnumerable<Guid> memberIds, CancellationToken cancellationToken = default)
    {
        var memberIdList = memberIds.ToList();
        
        return await PrepareQuery(_dbSet)
            .Where(mf => memberIdList.Contains(mf.MemberId))
            .OrderByDescending(mf => mf.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasFeatureAsync(Guid memberId, string featureKey, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        
        return await PrepareQuery(_dbSet)
            .AnyAsync(mf => mf.MemberId == memberId &&
                           mf.FeatureKey == featureKey &&
                           mf.IsActive &&
                           (mf.ValidFrom == null || mf.ValidFrom <= now) &&
                           (mf.ValidTo == null || mf.ValidTo > now), cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetExpiringFeaturesByMemberIdAsync(Guid memberId, TimeSpan timeThreshold, CancellationToken cancellationToken = default)
    {
        var cutoffTime = DateTimeOffset.UtcNow.Add(timeThreshold);
        
        return await PrepareQuery(_dbSet)
            .Where(mf => mf.MemberId == memberId &&
                        mf.IsActive &&
                        mf.ValidTo.HasValue &&
                        mf.ValidTo.Value <= cutoffTime &&
                        mf.ValidTo.Value > DateTimeOffset.UtcNow)
            .OrderByDescending(mf => mf.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetExpiringFeaturesAsync(TimeSpan timeThreshold, CancellationToken cancellationToken = default)
    {
        var cutoffTime = DateTimeOffset.UtcNow.Add(timeThreshold);
        
        return await PrepareQuery(_dbSet)
            .Where(mf => mf.IsActive &&
                        mf.ValidTo.HasValue &&
                        mf.ValidTo.Value <= cutoffTime &&
                        mf.ValidTo.Value > DateTimeOffset.UtcNow)
            .OrderByDescending(mf => mf.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetByAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(mf => mf.AssignedBy == assignedBy)
            .OrderByDescending(mf => mf.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet).AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(mf => mf.AssignedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(mf => mf.AssignedAt <= toDate.Value);
        }

        return await query
            .OrderByDescending(mf => mf.AssignedAt)
            .ToListAsync(cancellationToken);
    }
}
