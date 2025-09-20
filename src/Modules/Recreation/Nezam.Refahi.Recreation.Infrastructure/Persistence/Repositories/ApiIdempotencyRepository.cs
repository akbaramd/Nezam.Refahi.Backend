using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for API idempotency management
/// </summary>
public class ApiIdempotencyRepository : EfRepository<RecreationDbContext, ApiIdempotency, Guid>, IApiIdempotencyRepository
{
    public ApiIdempotencyRepository(RecreationDbContext context) : base(context)
    {
    }

    public async Task<ApiIdempotency?> GetByKeyAsync(string tenantId, string endpoint, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(ai => 
                ai.TenantId == tenantId && 
                ai.Endpoint == endpoint && 
                ai.IdempotencyKey == idempotencyKey, 
                cancellationToken);
    }

    public async Task<bool> ExistsAndNotExpiredAsync(string tenantId, string endpoint, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .AnyAsync(ai => 
                ai.TenantId == tenantId && 
                ai.Endpoint == endpoint && 
                ai.IdempotencyKey == idempotencyKey &&
                ai.ExpiresAt > now, 
                cancellationToken);
    }

    public async Task<(ApiIdempotency Record, bool IsNew)> GetOrCreateAsync(ApiIdempotency newRecord, CancellationToken cancellationToken = default)
    {
        // Try to get existing record first
        var existing = await GetByKeyAsync(
            newRecord.TenantId ?? "default", 
            newRecord.Endpoint, 
            newRecord.IdempotencyKey, 
            cancellationToken);

        if (existing != null)
        {
            // Check if expired
            if (existing.IsExpired)
            {
                // Remove expired record and create new one
                _dbSet.Remove(existing);
                await _dbSet.AddAsync(newRecord, cancellationToken);
                return (newRecord, true);
            }
            
            return (existing, false);
        }

        // Create new record
        await _dbSet.AddAsync(newRecord, cancellationToken);
        return (newRecord, true);
    }

    public async Task<int> CleanupExpiredAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        var expiredRecords = await PrepareQuery(_dbSet)
            .Where(ai => ai.ExpiresAt < cutoffDate)
            .ToListAsync(cancellationToken);

        if (expiredRecords.Any())
        {
            _dbSet.RemoveRange(expiredRecords);
            return expiredRecords.Count;
        }

        return 0;
    }

    public async Task<IEnumerable<ApiIdempotency>> GetByUserAsync(string tenantId, string userId, DateTime? fromDate = null, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet)
            .Where(ai => ai.TenantId == tenantId && ai.UserId == userId);

        if (fromDate.HasValue)
        {
            query = query.Where(ai => ai.CreatedAt >= fromDate.Value);
        }

        return await query
            .OrderByDescending(ai => ai.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteExpiredRecordsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        var expiredRecords = await PrepareQuery(_dbSet)
            .Where(ai => ai.ExpiresAt <= cutoffDate)
            .ToListAsync(cancellationToken);

        if (expiredRecords.Any())
        {
            _dbSet.RemoveRange(expiredRecords);
            return expiredRecords.Count;
        }

        return 0;
    }
}
