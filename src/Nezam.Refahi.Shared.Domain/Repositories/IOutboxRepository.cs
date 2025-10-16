using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Shared.Domain.Entities;

namespace Nezam.Refahi.Shared.Domain.Repositories;

/// <summary>
/// Repository for managing outbox messages with comprehensive failure handling
/// </summary>
public interface IOutboxRepository : IRepository<OutboxMessage, Guid>
{
    /// <summary>
    /// Adds a new outbox message
    /// </summary>
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple outbox messages in a batch
    /// </summary>
    Task AddRangeAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing outbox message
    /// </summary>
    Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unprocessed outbox messages
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unprocessed messages by event type
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesByTypeAsync(string eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed messages by event type
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetFailedMessagesByTypeAsync(string eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets DLQ messages
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetDlqMessagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets DLQ messages by event type
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetDlqMessagesByTypeAsync(string eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets DLQ messages older than specified date
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetDlqMessagesOlderThanAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as processed
    /// </summary>
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as failed and increments retry count
    /// </summary>
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed messages for retry
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetFailedMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a single integration event to the outbox and triggers processing
    /// </summary>
    Task PublishMessageAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes multiple integration events to the outbox and triggers processing
    /// </summary>
    Task PublishMessagesAsync<T>(IEnumerable<T> integrationEvents, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Gets processed messages older than the specified date
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetProcessedMessagesOlderThanAsync(DateTime cutoffDate, int batchSize = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed messages older than the specified date
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetFailedMessagesOlderThanAsync(DateTime cutoffDate, int batchSize = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes messages by their IDs
    /// </summary>
    Task<int> DeleteMessagesAsync(IEnumerable<Guid> messageIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets messages by idempotency key
    /// </summary>
    Task<OutboxMessage?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets messages by correlation ID
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets messages by aggregate ID
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetByAggregateIdAsync(Guid aggregateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics about outbox messages
    /// </summary>
    Task<OutboxStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Statistics about outbox messages
/// </summary>
public class OutboxStatistics
{
    public int TotalMessages { get; set; }
    public int UnprocessedMessages { get; set; }
    public int ProcessedMessages { get; set; }
    public int FailedMessages { get; set; }
    public int DlqMessages { get; set; }
    public int PoisonMessages { get; set; }
    public Dictionary<string, int> MessagesByType { get; set; } = new();
    public Dictionary<string, int> MessagesByStatus { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
