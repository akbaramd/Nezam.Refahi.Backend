using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Shared.Domain.Services;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Shared.Infrastructure.Services;

/// <summary>
/// Implementation of the idempotency service using Entity Framework
/// </summary>
public class IdempotencyService : IIdempotencyService
{
    private readonly AppDbContext _context;
    private readonly ILogger<IdempotencyService> _logger;

    public IdempotencyService(
        AppDbContext context,
        ILogger<IdempotencyService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Checks if an event with the given idempotency key has already been processed
    /// </summary>
    public async Task<bool> IsEventProcessedAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            _logger.LogWarning("Idempotency key is null or empty");
            return false;
        }

        try
        {
            var existingRecord = await _context.Set<EventIdempotency>()
                .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);

            var isProcessed = existingRecord?.IsProcessed ?? false;
            
            _logger.LogDebug("Idempotency check for key {IdempotencyKey}: {IsProcessed}", 
                idempotencyKey, isProcessed);

            return isProcessed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking idempotency for key {IdempotencyKey}", idempotencyKey);
            // In case of error, assume not processed to allow retry
            return false;
        }
    }

    /// <summary>
    /// Marks an event with the given idempotency key as processed
    /// </summary>
    public async Task MarkEventAsProcessedAsync(string idempotencyKey, Guid? aggregateId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            _logger.LogWarning("Cannot mark event as processed: idempotency key is null or empty");
            return;
        }

        try
        {
            var existingRecord = await _context.Set<EventIdempotency>()
                .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);

            if (existingRecord != null)
            {
                // Update existing record
                existingRecord.IsProcessed = true;
                existingRecord.ProcessedAt = DateTime.UtcNow;
                existingRecord.AggregateId = aggregateId;
                _context.Set<EventIdempotency>().Update(existingRecord);
            }
            else
            {
                // Create new record
                var newRecord = new EventIdempotency
                {
                    IdempotencyKey = idempotencyKey,
                    AggregateId = aggregateId,
                    IsProcessed = true,
                    ProcessedAt = DateTime.UtcNow
                };
                await _context.Set<EventIdempotency>().AddAsync(newRecord, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogDebug("Marked event as processed for idempotency key {IdempotencyKey}", idempotencyKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking event as processed for key {IdempotencyKey}", idempotencyKey);
            throw;
        }
    }

    /// <summary>
    /// Gets processing status of an event
    /// </summary>
    public async Task<EventProcessingStatus?> GetEventStatusAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            _logger.LogWarning("Idempotency key is null or empty");
            return null;
        }

        try
        {
            var existingRecord = await _context.Set<EventIdempotency>()
                .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);

            if (existingRecord == null)
            {
                _logger.LogDebug("No idempotency record found for key {IdempotencyKey}", idempotencyKey);
                return null;
            }

            var status = new EventProcessingStatus
            {
                IdempotencyKey = existingRecord.IdempotencyKey,
                AggregateId = existingRecord.AggregateId,
                IsProcessed = existingRecord.IsProcessed,
                ProcessedAt = existingRecord.ProcessedAt,
                Error = existingRecord.Error,
                RetryCount = existingRecord.RetryCount,
                CreatedAt = existingRecord.CreatedAt
            };

            _logger.LogDebug("Retrieved event status for key {IdempotencyKey}: {IsProcessed}", 
                idempotencyKey, status.IsProcessed);

            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event status for key {IdempotencyKey}", idempotencyKey);
            throw;
        }
    }

    /// <summary>
    /// Cleans up old idempotency records to prevent database bloat
    /// </summary>
    public async Task<int> CleanupOldRecordsAsync(int retentionDays = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
            
            var oldRecords = await _context.Set<EventIdempotency>()
                .Where(x => x.ProcessedAt < cutoffDate)
                .ToListAsync(cancellationToken);

            if (oldRecords.Any())
            {
                _context.Set<EventIdempotency>().RemoveRange(oldRecords);
                await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Cleaned up {Count} old idempotency records older than {CutoffDate}", 
                    oldRecords.Count, cutoffDate);
            }

            return oldRecords.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old idempotency records");
            throw;
        }
    }
}
