using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Shared.Domain.Entities;
using Nezam.Refahi.Shared.Domain.Repositories;
using Nezam.Refahi.Shared.Infrastructure.Outbox;
using Nezam.Refahi.Shared.Infrastructure.Persistence;
using System.Text.Json;

namespace Nezam.Refahi.Shared.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for OutboxMessage entity with publisher integration
/// </summary>
public class OutboxRepository : EfRepository<AppDbContext, OutboxMessage, Guid>, IOutboxRepository
{
    private readonly AppDbContext _context;

    public OutboxRepository(AppDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        // Guard: ensure Content has JSON
        if (string.IsNullOrWhiteSpace(message.Content))
        {
            // Serialize a minimal placeholder if missing to avoid poison loop; real serialization should happen at publisher
            message.Content = JsonSerializer.Serialize(new { });
        }

        await _context.OutboxMessages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken = default)
    {
        if (messages == null)
            throw new ArgumentNullException(nameof(messages));

        foreach (var message in messages)
        {
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                message.Content = JsonSerializer.Serialize(new { });
            }
        }

        await _context.OutboxMessages.AddRangeAsync(messages, cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.ProcessedOn == null && 
                       x.RetryCount < x.MaxRetries && 
                       !x.IsPoisonMessage &&
                       (x.NextRetryAt == null || DateTime.UtcNow >= x.NextRetryAt))
            .OrderBy(x => x.OccurredOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _dbSet.FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);
        if (message != null)
        {
            message.ProcessedOn = DateTime.UtcNow;
            _dbSet.Update(message);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default)
    {
        var message = await _dbSet.FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);
        if (message != null)
        {
            message.RetryCount++;
            message.Error = error;
            if (message.RetryCount >= message.MaxRetries)
            {
                message.ProcessedOn = DateTime.UtcNow; // Mark as processed with error after max retries
            }
            _dbSet.Update(message);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<OutboxMessage>> GetFailedMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.ProcessedOn == null && x.RetryCount >= x.MaxRetries)
            .OrderBy(x => x.OccurredOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

   
  public async Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        _context.OutboxMessages.Update(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a message to the outbox (repository only saves, no processing)
    /// </summary>
    public async Task PublishMessageAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : class
    {
        var eventType = typeof(T);
        var message = OutboxMessage.Create(
            integrationEvent,
            aggregateId: null, // Will be set by the event handler if needed
            correlationId: null, // Will be set by the event handler if needed
            idempotencyKey: null, // Will be set by the event handler if needed
            schemaVersion: 1
        );

        await AddAsync(message, cancellationToken);
    }

    /// <summary>
    /// Adds multiple messages to the outbox (repository only saves, no processing)
    /// </summary>
    public async Task PublishMessagesAsync<T>(IEnumerable<T> integrationEvents, CancellationToken cancellationToken = default) where T : class
    {
        var eventType = typeof(T);
        var messages = integrationEvents.Select(integrationEvent => OutboxMessage.Create(
            integrationEvent,
            aggregateId: null, // Will be set by the event handler if needed
            correlationId: null, // Will be set by the event handler if needed
            idempotencyKey: null, // Will be set by the event handler if needed
            schemaVersion: 1
        )).ToList();

        await AddRangeAsync(messages, cancellationToken);
    }

    /// <summary>
    /// Gets processed messages older than the specified date
    /// </summary>
    public async Task<IEnumerable<OutboxMessage>> GetProcessedMessagesOlderThanAsync(DateTime cutoffDate, int batchSize = 1000, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.ProcessedOn.HasValue && x.ProcessedOn.Value < cutoffDate)
            .OrderBy(x => x.ProcessedOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets failed messages older than the specified date
    /// </summary>
    public async Task<IEnumerable<OutboxMessage>> GetFailedMessagesOlderThanAsync(DateTime cutoffDate, int batchSize = 1000, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.ProcessedOn.HasValue && x.RetryCount >= x.MaxRetries && x.ProcessedOn.Value < cutoffDate)
            .OrderBy(x => x.ProcessedOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes messages by their IDs
    /// </summary>
    public async Task<int> DeleteMessagesAsync(IEnumerable<Guid> messageIds, CancellationToken cancellationToken = default)
    {
        if (messageIds == null || !messageIds.Any())
            return 0;

        var messageIdList = messageIds.ToList();
        var messagesToDelete = await _dbSet
            .Where(x => messageIdList.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (messagesToDelete.Any())
        {
            _dbSet.RemoveRange(messagesToDelete);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return messagesToDelete.Count;
    }

    /// <summary>
    /// Gets unprocessed messages by event type
    /// </summary>
    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesByTypeAsync(string eventType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.Type == eventType && x.ProcessedOn == null && x.RetryCount < x.MaxRetries)
            .OrderBy(x => x.OccurredOn)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets failed messages by event type
    /// </summary>
    public async Task<IEnumerable<OutboxMessage>> GetFailedMessagesByTypeAsync(string eventType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.Type == eventType && x.ProcessedOn == null && x.RetryCount >= x.MaxRetries)
            .OrderBy(x => x.OccurredOn)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets DLQ messages
    /// </summary>
    public async Task<IEnumerable<OutboxMessage>> GetDlqMessagesAsync(CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.MovedToDlqAt.HasValue)
            .OrderBy(x => x.MovedToDlqAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets DLQ messages by event type
    /// </summary>
    public async Task<IEnumerable<OutboxMessage>> GetDlqMessagesByTypeAsync(string eventType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.Type == eventType && x.MovedToDlqAt.HasValue)
            .OrderBy(x => x.MovedToDlqAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets DLQ messages older than specified date
    /// </summary>
    public async Task<IEnumerable<OutboxMessage>> GetDlqMessagesOlderThanAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.MovedToDlqAt.HasValue && x.MovedToDlqAt.Value < cutoffDate)
            .OrderBy(x => x.MovedToDlqAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets messages by idempotency key
    /// </summary>
    public async Task<OutboxMessage?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    /// <summary>
    /// Gets messages by correlation ID
    /// </summary>
    public async Task<IEnumerable<OutboxMessage>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.CorrelationId == correlationId)
            .OrderBy(x => x.OccurredOn)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets messages by aggregate ID
    /// </summary>
    public async Task<IEnumerable<OutboxMessage>> GetByAggregateIdAsync(Guid aggregateId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.AggregateId == aggregateId)
            .OrderBy(x => x.OccurredOn)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets statistics about outbox messages
    /// </summary>
    public async Task<OutboxStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var allMessages = await PrepareQuery(_dbSet).ToListAsync(cancellationToken);
        
        var statistics = new OutboxStatistics
        {
            TotalMessages = allMessages.Count,
            UnprocessedMessages = allMessages.Count(x => x.ProcessedOn == null && x.RetryCount < x.MaxRetries),
            ProcessedMessages = allMessages.Count(x => x.ProcessedOn.HasValue),
            FailedMessages = allMessages.Count(x => x.ProcessedOn == null && x.RetryCount >= x.MaxRetries),
            DlqMessages = allMessages.Count(x => x.MovedToDlqAt.HasValue),
            PoisonMessages = allMessages.Count(x => x.IsPoisonMessage),
            MessagesByType = allMessages.GroupBy(x => x.Type)
                .ToDictionary(g => g.Key, g => g.Count()),
            MessagesByStatus = allMessages.GroupBy(x => GetMessageStatus(x))
                .ToDictionary(g => g.Key, g => g.Count()),
            GeneratedAt = DateTime.UtcNow
        };

        return statistics;
    }

    private static string GetMessageStatus(OutboxMessage message)
    {
        if (message.IsPoisoned)
            return "Poisoned";
        if (message.IsInDlq)
            return "DLQ";
        if (message.IsProcessed)
            return "Processed";
        if (message.IsFailed)
            return "Failed";
        return "Unprocessed";
    }
}
