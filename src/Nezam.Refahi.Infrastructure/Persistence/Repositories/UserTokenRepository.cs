using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for UserToken entity operations
    /// </summary>
    public class UserTokenRepository : GenericRepository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Gets a token by its value and type
        /// </summary>
        /// <param name="tokenValue">The token value</param>
        /// <param name="tokenType">The token type</param>
        /// <returns>The token if found, null otherwise</returns>
        public async Task<UserToken?> GetByTokenValueAsync(string tokenValue, string tokenType)
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.TokenValue == tokenValue && t.TokenType == tokenType);
        }

        /// <summary>
        /// Gets all active tokens for a user
        /// </summary>
        /// <param name="userId">The user's ID</param>
        /// <param name="tokenType">Optional token type filter</param>
        /// <returns>Collection of active tokens</returns>
        public async Task<IEnumerable<UserToken>> GetActiveTokensForUserAsync(Guid userId, string? tokenType = null)
        {
            var query = _dbSet
                .Where(t => t.UserId == userId && !t.IsUsed && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

            if (!string.IsNullOrEmpty(tokenType))
            {
                query = query.Where(t => t.TokenType == tokenType);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Revokes all tokens of a specific type for a user
        /// </summary>
        /// <param name="userId">The user's ID</param>
        /// <param name="tokenType">The token type to revoke</param>
        /// <returns>Number of tokens revoked</returns>
        public async Task<int> RevokeAllUserTokensOfTypeAsync(Guid userId, string tokenType)
        {
            var tokens = await _dbSet
                .Where(t => t.UserId == userId && t.TokenType == tokenType && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoke();
            }

            await _dbContext.SaveChangesAsync();
            return tokens.Count;
        }

        /// <summary>
        /// Cleans up expired tokens
        /// </summary>
        /// <returns>Number of tokens removed</returns>
        public async Task<int> CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _dbSet
                .Where(t => t.ExpiresAt < DateTime.UtcNow.AddDays(-1)) // Keep tokens for 1 day after expiration for audit purposes
                .ToListAsync();

            _dbSet.RemoveRange(expiredTokens);
            await _dbContext.SaveChangesAsync();
            
            return expiredTokens.Count;
        }
    }
}
