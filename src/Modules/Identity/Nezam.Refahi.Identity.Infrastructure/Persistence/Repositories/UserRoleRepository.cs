using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Infrastructure.Persistence;


namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IUserRoleRepository
/// </summary>
public class UserRoleRepository : EfRepository<IdentityDbContext, UserRole, Guid>, IUserRoleRepository
{
    public UserRoleRepository(IdentityDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(ur => ur.UserId == userId)
            .OrderBy(ur => ur.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(ur => ur.UserId == userId && ur.IsActive && (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow))
            .OrderBy(ur => ur.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(ur => ur.RoleId == roleId)
            .OrderBy(ur => ur.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetActiveByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(ur => ur.RoleId == roleId && ur.IsActive && (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow))
            .OrderBy(ur => ur.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken:cancellationToken);
    }

    public async Task<UserRole?> GetActiveByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive && (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow), cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetExpiredAsync(CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(ur => ur.ExpiresAt.HasValue && ur.ExpiresAt <= DateTime.UtcNow)
            .OrderBy(ur => ur.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetExpiringSoonAsync(TimeSpan timeThreshold, CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.Add(timeThreshold);
        return await PrepareQuery(_dbSet)
            .Where(ur => ur.IsActive && ur.ExpiresAt.HasValue && ur.ExpiresAt <= thresholdDate && ur.ExpiresAt > DateTime.UtcNow)
            .OrderBy(ur => ur.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(ur => ur.AssignedBy == assignedBy)
            .OrderBy(ur => ur.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetByExpirationRangeAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(ur => ur.ExpiresAt >= fromDate);

        if (toDate.HasValue)
            query = query.Where(ur => ur.ExpiresAt <= toDate);

        return await query
            .OrderBy(ur => ur.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .CountAsync(ur => ur.UserId == userId && ur.IsActive && (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow), cancellationToken:cancellationToken);
    }

    public async Task<int> GetActiveCountByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .CountAsync(ur => ur.RoleId == roleId && ur.IsActive && (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow), cancellationToken:cancellationToken);
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken:cancellationToken);
    }

    public async Task<bool> UserHasActiveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive && (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow), cancellationToken:cancellationToken);
    }

    protected override IQueryable<UserRole> PrepareQuery(IQueryable<UserRole> query)
    {
        query = query.Include(ur => ur.Role).Include(ur => ur.User);
        return base.PrepareQuery(query);
    }
}
