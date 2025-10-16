using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.ReadModels;
using Nezam.Refahi.Facilities.Application.Services;

namespace Nezam.Refahi.Facilities.Application.Services;

/// <summary>
/// Service for managing Member snapshots in Facilities context
/// This acts as an Anti-Corruption Layer (ACL) for Membership context data
/// </summary>
public interface IMemberSnapshotService
{
    /// <summary>
    /// Gets a member snapshot by External User ID
    /// </summary>
    /// <param name="externalUserId">The external user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Member snapshot if found, null otherwise</returns>
    Task<MemberSnapshot?> GetSnapshotByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a member snapshot by Member ID
    /// </summary>
    /// <param name="memberId">The member ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Member snapshot if found, null otherwise</returns>
    Task<MemberSnapshot?> GetSnapshotByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates or updates a member snapshot
    /// </summary>
    /// <param name="memberSnapshot">The member snapshot to create/update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task CreateOrUpdateSnapshotAsync(MemberSnapshot memberSnapshot, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all member snapshots
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of member snapshots</returns>
    Task<IEnumerable<MemberSnapshot>> GetAllSnapshotsAsync(CancellationToken cancellationToken = default);
}
