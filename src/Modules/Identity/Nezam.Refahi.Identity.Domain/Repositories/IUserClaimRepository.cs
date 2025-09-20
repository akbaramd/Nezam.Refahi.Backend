using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Repository interface for UserClaim entity
/// </summary>
public interface IUserClaimRepository : IRepository<UserClaim, Guid>
{
    /// <summary>
    /// Gets user claims by user ID
    /// </summary>
    Task<IEnumerable<UserClaim>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active user claims by user ID
    /// </summary>
    Task<IEnumerable<UserClaim>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user claims by claim type
    /// </summary>
    Task<IEnumerable<UserClaim>> GetByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active user claims by claim type
    /// </summary>
    Task<IEnumerable<UserClaim>> GetActiveByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user claims by claim type and value
    /// </summary>
    Task<IEnumerable<UserClaim>> GetByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active user claims by claim type and value
    /// </summary>
    Task<IEnumerable<UserClaim>> GetActiveByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific user claim
    /// </summary>
    Task<UserClaim?> GetByUserAndClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active user claim
    /// </summary>
    Task<UserClaim?> GetActiveByUserAndClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired user claims
    /// </summary>
    Task<IEnumerable<UserClaim>> GetExpiredAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user claims that will expire soon
    /// </summary>
    Task<IEnumerable<UserClaim>> GetExpiringSoonAsync(TimeSpan timeThreshold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user claims assigned by a specific user
    /// </summary>
    Task<IEnumerable<UserClaim>> GetAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user claims with expiration date range
    /// </summary>
    Task<IEnumerable<UserClaim>> GetByExpirationRangeAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of active user claims for a user
    /// </summary>
    Task<int> GetActiveCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of user claims by claim type
    /// </summary>
    Task<int> GetCountByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of active user claims by claim type
    /// </summary>
    Task<int> GetActiveCountByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific claim
    /// </summary>
    Task<bool> UserHasClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has an active claim
    /// </summary>
    Task<bool> UserHasActiveClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default);

   
}
