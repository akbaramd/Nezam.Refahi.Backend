using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// Production-grade repository implementation for UserToken entity operations
/// Implements DDD + EF Core best practices for token management
/// </summary>
public class UserTokenRepository : EfRepository<IdentityDbContext, UserToken, Guid>, IUserTokenRepository
{
    private readonly ILogger<UserTokenRepository>? _logger;

    public UserTokenRepository(IdentityDbContext dbContext, ILogger<UserTokenRepository>? logger = null) 
        : base(dbContext)
    {
        _logger = logger;
    }

    // ========================================================================
    // Token Retrieval Operations
    // ========================================================================

    public async Task<UserToken?> GetByTokenValueAsync(string tokenValue, string tokenType)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(t => t.TokenValue == tokenValue && t.TokenType == tokenType);
    }

    public async Task<IEnumerable<UserToken>> GetActiveTokensForUserAsync(Guid userId, string? tokenType = null)
    {
        var query = PrepareQuery(_dbSet)
            .Where(t => t.UserId == userId && !t.IsUsed && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

        if (!string.IsNullOrEmpty(tokenType))
        {
            query = query.Where(t => t.TokenType == tokenType);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<UserToken>> GetTokensBySessionFamilyAsync(Guid sessionFamilyId)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.SessionFamilyId == sessionFamilyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserToken>> GetRefreshTokensByDeviceAsync(Guid userId, string deviceFingerprint)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.UserId == userId && 
                       t.TokenType == "RefreshToken" && 
                       t.DeviceFingerprint == deviceFingerprint &&
                       !t.IsRevoked)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserToken>> GetAllActiveTokensByTypeAsync(string tokenType)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.TokenType == tokenType && 
                       !t.IsUsed && 
                       !t.IsRevoked && 
                       t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    // ========================================================================
    // Token Revocation Operations
    // ========================================================================

    public async Task<int> RevokeAllUserTokensOfTypeAsync(Guid userId, string tokenType, bool isSoftDelete = true, bool savedChanges = false)
    {
        var tokens = await PrepareQuery(_dbSet)
            .Where(t => t.UserId == userId && t.TokenType == tokenType && !t.IsRevoked)
            .ToListAsync();

        if (isSoftDelete)
        {
            foreach (var token in tokens)
            {
                token.Revoke();
            }
        }
        else
        {
            _dbSet.RemoveRange(tokens);
        }

        if (savedChanges)
        {
            await _dbContext.SaveChangesAsync();
        }

        _logger?.LogDebug("Revoked {Count} {TokenType} tokens for user {UserId}", tokens.Count, tokenType, userId);
        return tokens.Count;
    }

    public async Task<int> RevokeSessionFamilyAsync(Guid sessionFamilyId, bool savedChanges = false)
    {
        var tokens = await PrepareQuery(_dbSet)
            .Where(t => t.SessionFamilyId == sessionFamilyId && !t.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Revoke();
        }

        if (savedChanges)
        {
            await _dbContext.SaveChangesAsync();
        }

        _logger?.LogWarning("Revoked {Count} tokens in session family {SessionFamilyId} due to reuse detection", 
            tokens.Count, sessionFamilyId);
        return tokens.Count;
    }

    public async Task<int> RevokeDeviceRefreshTokensAsync(Guid userId, string deviceFingerprint, bool savedChanges = false)
    {
        var tokens = await PrepareQuery(_dbSet)
            .Where(t => t.UserId == userId && 
                       t.TokenType == "RefreshToken" && 
                       t.DeviceFingerprint == deviceFingerprint &&
                       !t.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Revoke();
        }

        if (savedChanges)
        {
            await _dbContext.SaveChangesAsync();
        }

        _logger?.LogInformation("Revoked {Count} refresh tokens for user {UserId} on device {DeviceFingerprint}", 
            tokens.Count, userId, deviceFingerprint);
        return tokens.Count;
    }

    public async Task<bool> RevokeJwtByIdAsync(string jwtId, bool savedChanges = false)
    {
        var token = await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(t => t.TokenType == "AccessToken" && t.TokenValue == jwtId);

        if (token == null)
            return false;

        token.Revoke();

        if (savedChanges)
        {
            await _dbContext.SaveChangesAsync();
        }

        _logger?.LogInformation("Revoked JWT {JwtId}", jwtId);
        return true;
    }

    // ========================================================================
    // Token Cleanup Operations
    // ========================================================================

    public async Task<int> CleanupExpiredTokensAsync(bool savedChanges = false)
    {
        var expiredTokens = await PrepareQuery(_dbSet)
            .Where(t => t.ExpiresAt < DateTime.UtcNow.AddDays(-1)) // Keep tokens for 1 day after expiration for audit purposes
            .ToListAsync();

        _dbSet.RemoveRange(expiredTokens);

        if (savedChanges)
        {
            await _dbContext.SaveChangesAsync();
        }

        _logger?.LogDebug("Cleaned up {Count} expired tokens", expiredTokens.Count);
        return expiredTokens.Count;
    }

    public async Task<int> CleanupIdleTokensAsync(int idleTimeoutDays = 7, bool savedChanges = false)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-idleTimeoutDays);
        var idleTokens = await PrepareQuery(_dbSet)
            .Where(t => t.LastUsedAt.HasValue && 
                       t.LastUsedAt < cutoffDate && 
                       t.TokenType == "RefreshToken" &&
                       !t.IsRevoked)
            .ToListAsync();

        foreach (var token in idleTokens)
        {
            token.Revoke();
        }

        if (savedChanges)
        {
            await _dbContext.SaveChangesAsync();
        }

        _logger?.LogDebug("Cleaned up {Count} idle tokens (idle timeout: {IdleTimeoutDays} days)", 
            idleTokens.Count, idleTimeoutDays);
        return idleTokens.Count;
    }

    public async Task<int> CleanupOldRevokedTokensAsync(int olderThanDays = 30, bool savedChanges = false)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
        var oldRevokedTokens = await PrepareQuery(_dbSet)
            .Where(t => t.IsRevoked )
            .ToListAsync();

        _dbSet.RemoveRange(oldRevokedTokens);

        if (savedChanges)
        {
            await _dbContext.SaveChangesAsync();
        }

        _logger?.LogDebug("Cleaned up {Count} old revoked tokens (older than {OlderThanDays} days)", 
            oldRevokedTokens.Count, olderThanDays);
        return oldRevokedTokens.Count;
    }

    // ========================================================================
    // Token Statistics and Monitoring
    // ========================================================================

    public async Task<TokenStatistics> GetTokenStatisticsAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var sevenDaysAgo = now.AddDays(-7);

        var tokens = await PrepareQuery(_dbSet)
            .Where(t => t.UserId == userId)
            .ToListAsync();

        var activeTokens = tokens.Where(t => !t.IsUsed && !t.IsRevoked && t.ExpiresAt > now).ToList();
        var revokedTokens = tokens.Where(t => t.IsRevoked).ToList();
        var expiredTokens = tokens.Where(t => t.ExpiresAt <= now).ToList();
        var idleTokens = tokens.Where(t => t.LastUsedAt.HasValue && t.LastUsedAt < sevenDaysAgo && t.TokenType == "RefreshToken").ToList();

        return new TokenStatistics
        {
            TotalActiveTokens = activeTokens.Count,
            ActiveRefreshTokens = activeTokens.Count(t => t.TokenType == "RefreshToken"),
            ActiveAccessTokens = activeTokens.Count(t => t.TokenType == "AccessToken"),
            RevokedTokens = revokedTokens.Count,
            ExpiredTokens = expiredTokens.Count,
            IdleTokens = idleTokens.Count,
         
        };
    }

    public async Task<int> GetActiveSessionCountAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var sevenDaysAgo = now.AddDays(-7);

        return await PrepareQuery(_dbSet)
            .Where(t => t.UserId == userId && 
                       t.TokenType == "RefreshToken" && 
                       !t.IsUsed && 
                       !t.IsRevoked && 
                       t.ExpiresAt > now &&
                       (!t.LastUsedAt.HasValue || t.LastUsedAt > sevenDaysAgo))
            .CountAsync();
    }

    protected override IQueryable<UserToken> PrepareQuery(IQueryable<UserToken> query)
    {
        return base.PrepareQuery(query);
    }
}
