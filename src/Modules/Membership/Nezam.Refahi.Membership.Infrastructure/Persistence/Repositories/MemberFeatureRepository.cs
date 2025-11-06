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

    protected override IQueryable<MemberFeature> PrepareQuery(IQueryable<MemberFeature> query)
    {
        return query.Include(mf => mf.Member);
    }

    public async Task<IEnumerable<MemberFeature>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(mf => mf.MemberId == memberId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetByFeatureKeyAsync(string featureKey, CancellationToken cancellationToken = default)
    {
        return await FindAsync(mf => mf.FeatureKey == featureKey, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetByFeatureKeysAsync(IEnumerable<string> featureKeys, CancellationToken cancellationToken = default)
    {
        var featureKeyList = featureKeys.ToList();
        return await FindAsync(mf => featureKeyList.Contains(mf.FeatureKey), cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<MemberFeature>> GetByMemberIdsAsync(IEnumerable<Guid> memberIds, CancellationToken cancellationToken = default)
    {
        var memberIdList = memberIds.ToList();
        return await FindAsync(mf => memberIdList.Contains(mf.MemberId), cancellationToken: cancellationToken);
    }

    public async Task<bool> HasFeatureAsync(Guid memberId, string featureKey, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(mf => mf.MemberId == memberId && mf.FeatureKey == featureKey, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<string>> GetFeatureKeysByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(mf => mf.MemberId == memberId, cancellationToken: cancellationToken))
            .Select(mf => mf.FeatureKey)
            .ToList();
    }

    public async Task RemoveAllMemberFeaturesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var memberFeatures = (await FindAsync(mf => mf.MemberId == memberId, cancellationToken: cancellationToken)).ToList();

        if (memberFeatures.Any())
        {
            await DeleteRangeAsync(memberFeatures, cancellationToken: cancellationToken);
        }
    }
}
