using MCA.SharedKernel.Domain.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Repository interface for RefreshSession entities
/// </summary>
public interface IRefreshSessionRepository : IRepository<RefreshSession, Guid>
{
    /// <summary>
    /// Gets active refresh sessions for a user and client
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="deviceFingerprint">Optional device fingerprint</param>
    /// <returns>Collection of active refresh sessions</returns>
    Task<IEnumerable<RefreshSession>> GetActiveByUserAndClientAsync(Guid userId, string clientId, DeviceFingerprint? deviceFingerprint = null);

    /// <summary>
    /// Gets all active refresh sessions for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of active refresh sessions</returns>
    Task<IEnumerable<RefreshSession>> GetActiveByUserAsync(Guid userId);

    /// <summary>
    /// Gets a refresh session by token hash
    /// </summary>
    /// <param name="tokenHash">Token hash to search for</param>
    /// <returns>Refresh session if found, null otherwise</returns>
    Task<RefreshSession?> GetByTokenHashAsync(string tokenHash);

    /// <summary>
    /// Gets expired refresh sessions
    /// </summary>
    /// <param name="beforeDate">Date before which sessions are considered expired</param>
    /// <returns>Collection of expired refresh sessions</returns>
    Task<IEnumerable<RefreshSession>> GetExpiredAsync(DateTime beforeDate);

    /// <summary>
    /// Gets revoked refresh sessions
    /// </summary>
    /// <returns>Collection of revoked refresh sessions</returns>
    Task<IEnumerable<RefreshSession>> GetRevokedAsync();

    /// <summary>
    /// Gets refresh sessions by device fingerprint
    /// </summary>
    /// <param name="deviceFingerprint">Device fingerprint to filter by</param>
    /// <returns>Collection of refresh sessions for the specified device</returns>
    Task<IEnumerable<RefreshSession>> GetByDeviceFingerprintAsync(DeviceFingerprint deviceFingerprint);

    /// <summary>
    /// Gets the count of active refresh sessions for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Number of active refresh sessions</returns>
    Task<int> GetActiveCountByUserAsync(Guid userId);

    /// <summary>
    /// Gets the count of active refresh sessions for a user and client
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="clientId">Client ID</param>
    /// <returns>Number of active refresh sessions</returns>
    Task<int> GetActiveCountByUserAndClientAsync(Guid userId, string clientId);

    /// <summary>
    /// Revokes all refresh sessions for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="reason">Reason for revocation</param>
    /// <returns>Number of revoked sessions</returns>
    Task<int> RevokeAllByUserAsync(Guid userId, string reason);

    /// <summary>
    /// Revokes all refresh sessions for a user and client
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="reason">Reason for revocation</param>
    /// <returns>Number of revoked sessions</returns>
    Task<int> RevokeAllByUserAndClientAsync(Guid userId, string clientId, string reason);

    /// <summary>
    /// Deletes expired refresh sessions
    /// </summary>
    /// <param name="beforeDate">Date before which sessions are considered expired</param>
    /// <returns>Number of deleted sessions</returns>
    Task<int> DeleteExpiredAsync(DateTime beforeDate);

    /// <summary>
    /// Gets refresh sessions by creation date range
    /// </summary>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <returns>Collection of refresh sessions created in the specified range</returns>
    Task<IEnumerable<RefreshSession>> GetByCreationDateRangeAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets refresh sessions by last used date range
    /// </summary>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <returns>Collection of refresh sessions last used in the specified range</returns>
    Task<IEnumerable<RefreshSession>> GetByLastUsedDateRangeAsync(DateTime fromDate, DateTime toDate);
}
