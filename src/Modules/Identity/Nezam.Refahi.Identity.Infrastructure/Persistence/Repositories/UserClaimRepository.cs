using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Infrastructure.Persistence;


namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IUserClaimRepository
/// </summary>
public class UserClaimRepository : EfRepository<IdentityDbContext, UserClaim, Guid>, IUserClaimRepository
{
    public UserClaimRepository(IdentityDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserClaim>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(uc => uc.UserId == userId)
            .OrderBy(uc => uc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserClaim>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(uc => uc.UserId == userId && uc.IsActive && (uc.ExpiresAt == null || uc.ExpiresAt > DateTime.UtcNow))
            .OrderBy(uc => uc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserClaim>> GetByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(uc => uc.Claim.Type == claimType)
            .OrderBy(uc => uc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserClaim>> GetActiveByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(uc => uc.Claim.Type == claimType && uc.IsActive && (uc.ExpiresAt == null || uc.ExpiresAt > DateTime.UtcNow))
            .OrderBy(uc => uc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserClaim>> GetByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(uc => uc.Claim.Type == claimType && uc.Claim.Value == claimValue)
            .OrderBy(uc => uc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserClaim>> GetActiveByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(uc => uc.Claim.Type == claimType && uc.Claim.Value == claimValue && uc.IsActive && (uc.ExpiresAt == null || uc.ExpiresAt > DateTime.UtcNow))
            .OrderBy(uc => uc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserClaim?> GetByUserAndClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.Claim.Type == claimType && uc.Claim.Value == claimValue, cancellationToken:cancellationToken);
    }

    public async Task<UserClaim?> GetActiveByUserAndClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.Claim.Type == claimType && uc.Claim.Value == claimValue && uc.IsActive && (uc.ExpiresAt == null || uc.ExpiresAt > DateTime.UtcNow), cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<UserClaim>> GetExpiredAsync(CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(uc => uc.ExpiresAt.HasValue && uc.ExpiresAt <= DateTime.UtcNow)
            .OrderBy(uc => uc.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserClaim>> GetExpiringSoonAsync(TimeSpan timeThreshold, CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.Add(timeThreshold);
        return await PrepareQuery(_dbSet)
            .Where(uc => uc.IsActive && uc.ExpiresAt.HasValue && uc.ExpiresAt <= thresholdDate && uc.ExpiresAt > DateTime.UtcNow)
            .OrderBy(uc => uc.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserClaim>> GetAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(uc => uc.AssignedBy == assignedBy)
            .OrderBy(uc => uc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserClaim>> GetByExpirationRangeAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(uc => uc.ExpiresAt >= fromDate);

        if (toDate.HasValue)
            query = query.Where(uc => uc.ExpiresAt <= toDate);

        return await query
            .OrderBy(uc => uc.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .CountAsync(uc => uc.UserId == userId && uc.IsActive && (uc.ExpiresAt == null || uc.ExpiresAt > DateTime.UtcNow), cancellationToken:cancellationToken);
    }

    public async Task<int> GetCountByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .CountAsync(uc => uc.Claim.Type == claimType, cancellationToken:cancellationToken);
    }

    public async Task<int> GetActiveCountByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .CountAsync(uc => uc.Claim.Type == claimType && uc.IsActive && (uc.ExpiresAt == null || uc.ExpiresAt > DateTime.UtcNow), cancellationToken:cancellationToken);
    }

    public async Task<bool> UserHasClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(uc => uc.UserId == userId && uc.Claim.Type == claimType && uc.Claim.Value == claimValue, cancellationToken:cancellationToken);
    }

    public async Task<bool> UserHasActiveClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(uc => uc.UserId == userId && uc.Claim.Type == claimType && uc.Claim.Value == claimValue && uc.IsActive && (uc.ExpiresAt == null || uc.ExpiresAt > DateTime.UtcNow), cancellationToken:cancellationToken);
    }

    protected override IQueryable<UserClaim> PrepareQuery(IQueryable<UserClaim> query)
    {
        query = query.Include(uc => uc.User);
        return base.PrepareQuery(query);
    }
}
