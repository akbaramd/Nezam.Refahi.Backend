using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for API idempotency management
/// </summary>
public interface IApiIdempotencyRepository : IRepository<ApiIdempotency, Guid>
{
    /// <summary>
    /// Gets an idempotency record by composite key
    /// </summary>
    Task<ApiIdempotency?> GetByKeyAsync(string tenantId, string endpoint, string idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an idempotency key exists and is not expired
    /// </summary>
    Task<bool> ExistsAndNotExpiredAsync(string tenantId, string endpoint, string idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or gets existing idempotency record atomically
    /// </summary>
    Task<(ApiIdempotency Record, bool IsNew)> GetOrCreateAsync(ApiIdempotency newRecord, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up expired idempotency records
    /// </summary>
    Task<int> CleanupExpiredAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired idempotency records older than the specified date
    /// </summary>
    Task<int> DeleteExpiredRecordsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets idempotency records by user for audit purposes
    /// </summary>
    Task<IEnumerable<ApiIdempotency>> GetByUserAsync(string tenantId, string userId, DateTime? fromDate = null, CancellationToken cancellationToken = default);
}
