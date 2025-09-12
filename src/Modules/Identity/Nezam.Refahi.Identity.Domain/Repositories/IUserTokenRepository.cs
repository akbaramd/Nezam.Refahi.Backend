using MCA.SharedKernel.Domain.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Production-grade repository interface for UserToken entity operations
/// Implements DDD + EF Core best practices for token management
/// </summary>
public interface IUserTokenRepository : IRepository<UserToken,Guid>
{
    // ========================================================================
    // Token Retrieval Operations
    // ========================================================================
    
    /// <summary>
    /// Gets a token by its value and type
    /// </summary>
    /// <param name="tokenValue">The token value (hashed for refresh tokens, jti for access tokens)</param>
    /// <param name="tokenType">The token type</param>
    /// <returns>The token if found, null otherwise</returns>
    Task<UserToken?> GetByTokenValueAsync(string tokenValue, string tokenType);
    
    /// <summary>
    /// Gets all active tokens for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="tokenType">Optional token type filter</param>
    /// <returns>Collection of active tokens</returns>
    Task<IEnumerable<UserToken>> GetActiveTokensForUserAsync(Guid userId, string? tokenType = null);
    
    /// <summary>
    /// Gets refresh tokens for a specific session family
    /// </summary>
    /// <param name="sessionFamilyId">Session family ID</param>
    /// <returns>Collection of tokens in the session family</returns>
    Task<IEnumerable<UserToken>> GetTokensBySessionFamilyAsync(Guid sessionFamilyId);
    
    /// <summary>
    /// Gets refresh tokens for a specific device
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="deviceFingerprint">Device fingerprint</param>
    /// <returns>Collection of tokens for the device</returns>
    Task<IEnumerable<UserToken>> GetRefreshTokensByDeviceAsync(Guid userId, string deviceFingerprint);
    
    /// <summary>
    /// Gets all active tokens of a specific type across all users (for token lookup)
    /// </summary>
    /// <param name="tokenType">Token type to filter by</param>
    /// <returns>Collection of active tokens of the specified type</returns>
    Task<IEnumerable<UserToken>> GetAllActiveTokensByTypeAsync(string tokenType);
    
    // ========================================================================
    // Token Revocation Operations
    // ========================================================================
    
    /// <summary>
    /// Revokes all tokens of a specific type for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="tokenType">The token type to revoke</param>
    /// <param name="isSoftDelete">Whether to soft delete (revoke) or hard delete</param>
    /// <param name="savedChanges">Whether to save changes immediately</param>
    /// <returns>Number of tokens revoked</returns>
    Task<int> RevokeAllUserTokensOfTypeAsync(Guid userId, string tokenType, bool isSoftDelete = true, bool savedChanges = false);
    
    /// <summary>
    /// Revokes all refresh tokens in a session family (for reuse detection)
    /// </summary>
    /// <param name="sessionFamilyId">Session family ID to revoke</param>
    /// <param name="savedChanges">Whether to save changes immediately</param>
    /// <returns>Number of tokens revoked</returns>
    Task<int> RevokeSessionFamilyAsync(Guid sessionFamilyId, bool savedChanges = false);
    
    /// <summary>
    /// Revokes refresh tokens for a specific device
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="deviceFingerprint">Device fingerprint</param>
    /// <param name="savedChanges">Whether to save changes immediately</param>
    /// <returns>Number of tokens revoked</returns>
    Task<int> RevokeDeviceRefreshTokensAsync(Guid userId, string deviceFingerprint, bool savedChanges = false);
    
    /// <summary>
    /// Revokes a specific JWT by its jti claim
    /// </summary>
    /// <param name="jwtId">JWT ID to revoke</param>
    /// <param name="savedChanges">Whether to save changes immediately</param>
    /// <returns>True if token was found and revoked</returns>
    Task<bool> RevokeJwtByIdAsync(string jwtId, bool savedChanges = false);
    
    // ========================================================================
    // Token Cleanup Operations
    // ========================================================================
    
    /// <summary>
    /// Cleans up expired tokens
    /// </summary>
    /// <param name="savedChanges">Whether to save changes immediately</param>
    /// <returns>Number of tokens removed</returns>
    Task<int> CleanupExpiredTokensAsync(bool savedChanges = false);
    
    /// <summary>
    /// Cleans up tokens that have exceeded idle timeout
    /// </summary>
    /// <param name="idleTimeoutDays">Idle timeout in days</param>
    /// <param name="savedChanges">Whether to save changes immediately</param>
    /// <returns>Number of tokens removed</returns>
    Task<int> CleanupIdleTokensAsync(int idleTimeoutDays = 7, bool savedChanges = false);
    
    /// <summary>
    /// Cleans up old revoked tokens (for audit trail maintenance)
    /// </summary>
    /// <param name="olderThanDays">Remove revoked tokens older than this many days</param>
    /// <param name="savedChanges">Whether to save changes immediately</param>
    /// <returns>Number of tokens removed</returns>
    Task<int> CleanupOldRevokedTokensAsync(int olderThanDays = 30, bool savedChanges = false);
    
    // ========================================================================
    // Token Statistics and Monitoring
    // ========================================================================
    
    /// <summary>
    /// Gets token statistics for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Token statistics</returns>
    Task<TokenStatistics> GetTokenStatisticsAsync(Guid userId);
    
    /// <summary>
    /// Gets active session count for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Number of active sessions</returns>
    Task<int> GetActiveSessionCountAsync(Guid userId);
}