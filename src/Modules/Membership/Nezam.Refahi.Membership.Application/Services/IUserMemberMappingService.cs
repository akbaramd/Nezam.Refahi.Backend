using Nezam.Refahi.Membership.Application.ReadModels;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Application.Services;

/// <summary>
/// Service for reverse mapping between User and Member
/// Provides read-only access to User-Member relationships
/// </summary>
public interface IUserMemberMappingService
{
    /// <summary>
    /// Gets member information by user ID
    /// </summary>
    /// <param name="userId">The user ID from Identity context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User-Member mapping information</returns>
    Task<UserMemberReadModel?> GetMemberByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user information by member ID
    /// </summary>
    /// <param name="memberId">The member ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User-Member mapping information</returns>
    Task<UserMemberReadModel?> GetUserByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has an associated member
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has a member, false otherwise</returns>
    Task<bool> HasMemberAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user-member mappings
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of user-member mappings</returns>
    Task<IEnumerable<UserMemberReadModel>> GetAllMappingsAsync(CancellationToken cancellationToken = default);
}
