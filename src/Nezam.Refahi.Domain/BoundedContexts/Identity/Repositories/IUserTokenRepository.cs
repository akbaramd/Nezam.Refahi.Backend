using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;

/// <summary>
/// Repository interface for UserToken entity operations
/// </summary>
public interface IUserTokenRepository : IGenericRepository<UserToken>
{
    /// <summary>
    /// Gets a token by its value and type
    /// </summary>
    /// <param name="tokenValue">The token value</param>
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
    /// Revokes all tokens of a specific type for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="tokenType">The token type to revoke</param>
    /// <returns>Number of tokens revoked</returns>
    Task<int> RevokeAllUserTokensOfTypeAsync(Guid userId, string tokenType);
    
    /// <summary>
    /// Cleans up expired tokens
    /// </summary>
    /// <returns>Number of tokens removed</returns>
    Task<int> CleanupExpiredTokensAsync();
}
