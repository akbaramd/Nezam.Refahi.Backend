using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of refresh session repository interface using EF Core
/// </summary>
public class RefreshSessionRepository : EfRepository<IdentityDbContext, RefreshSession, Guid>, IRefreshSessionRepository
{
    public RefreshSessionRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<RefreshSession>> GetActiveByUserAndClientAsync(Guid userId, string clientId, DeviceFingerprint? deviceFingerprint = null)
    {
        var query = _dbContext.Set<RefreshSession>()
            .Where(rs => rs.UserId == userId && rs.ClientId == clientId && rs.IsActive);

        if (deviceFingerprint != null)
        {
            query = query.Where(rs => rs.DeviceFingerprint == deviceFingerprint);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<RefreshSession>> GetActiveByUserAsync(Guid userId)
    {
        return await _dbContext.Set<RefreshSession>()
            .Where(rs => rs.UserId == userId && rs.IsActive)
            .ToListAsync();
    }

    public async Task<RefreshSession?> GetByTokenHashAsync(string tokenHash)
    {
        return await _dbContext.Set<RefreshSession>()
            .FirstOrDefaultAsync(rs => rs.CurrentTokenHash.Hash == tokenHash);
    }

    public async Task<IEnumerable<RefreshSession>> GetExpiredAsync(DateTime beforeDate)
    {
        return await _dbContext.Set<RefreshSession>()
            .Where(rs => rs.CreatedAt < beforeDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<RefreshSession>> GetRevokedAsync()
    {
        return await _dbContext.Set<RefreshSession>()
            .Where(rs => rs.RevokedAt != null)
            .ToListAsync();
    }

    public async Task<IEnumerable<RefreshSession>> GetByDeviceFingerprintAsync(DeviceFingerprint deviceFingerprint)
    {
        return await _dbContext.Set<RefreshSession>()
            .Where(rs => rs.DeviceFingerprint == deviceFingerprint)
            .ToListAsync();
    }

    public async Task<int> GetActiveCountByUserAsync(Guid userId)
    {
        return await _dbContext.Set<RefreshSession>()
            .CountAsync(rs => rs.UserId == userId && rs.IsActive);
    }

    public async Task<int> GetActiveCountByUserAndClientAsync(Guid userId, string clientId)
    {
        return await _dbContext.Set<RefreshSession>()
            .CountAsync(rs => rs.UserId == userId && rs.ClientId == clientId && rs.IsActive);
    }

    public async Task<int> RevokeAllByUserAsync(Guid userId, string reason)
    {
        var sessions = await _dbContext.Set<RefreshSession>()
            .Where(rs => rs.UserId == userId && rs.IsActive)
            .ToListAsync();

        foreach (var session in sessions)
        {
            session.Revoke(reason);
        }

        return sessions.Count;
    }

    public async Task<int> RevokeAllByUserAndClientAsync(Guid userId, string clientId, string reason)
    {
        var sessions = await _dbContext.Set<RefreshSession>()
            .Where(rs => rs.UserId == userId && rs.ClientId == clientId && rs.IsActive)
            .ToListAsync();

        foreach (var session in sessions)
        {
            session.Revoke(reason);
        }

        return sessions.Count;
    }

    public async Task<int> DeleteExpiredAsync(DateTime beforeDate)
    {
        var expiredSessions = await _dbContext.Set<RefreshSession>()
            .Where(rs => rs.CreatedAt < beforeDate)
            .ToListAsync();

        _dbContext.Set<RefreshSession>().RemoveRange(expiredSessions);
        return expiredSessions.Count;
    }

    public async Task<IEnumerable<RefreshSession>> GetByCreationDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbContext.Set<RefreshSession>()
            .Where(rs => rs.CreatedAt >= fromDate && rs.CreatedAt <= toDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<RefreshSession>> GetByLastUsedDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbContext.Set<RefreshSession>()
            .Where(rs => rs.LastUsedAt >= fromDate && rs.LastUsedAt <= toDate)
            .ToListAsync();
    }
}
