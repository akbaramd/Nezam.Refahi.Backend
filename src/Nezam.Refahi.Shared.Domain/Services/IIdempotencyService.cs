using MCA.SharedKernel.Domain;
using Nezam.Refahi.Shared.Domain.Entities;

namespace Nezam.Refahi.Shared.Domain.Services;

/// <summary>
/// Service for managing idempotency in event processing
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
    /// Checks if an event has already been processed
    /// </summary>
    Task<bool> IsEventProcessedAsync(string idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an event as processed
    /// </summary>
    Task MarkEventAsProcessedAsync(string idempotencyKey, Guid? aggregateId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets processing status of an event
    /// </summary>
    Task<EventProcessingStatus?> GetEventStatusAsync(string idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up old idempotency records
    /// </summary>
    Task<int> CleanupOldRecordsAsync(int retentionDays = 30, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the processing status of an event
/// </summary>
public class EventProcessingStatus
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public Guid? AggregateId { get; set; }
    public bool IsProcessed { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Entity for tracking event idempotency
/// </summary>
public class EventIdempotency : Entity<Guid>
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public Guid? AggregateId { get; set; }
    public bool IsProcessed { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public EventIdempotency() : base(Guid.NewGuid())
    {
    }

    public static EventIdempotency Create(string idempotencyKey, Guid? aggregateId = null)
    {
        return new EventIdempotency
        {
            IdempotencyKey = idempotencyKey,
            AggregateId = aggregateId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsProcessed()
    {
        IsProcessed = true;
        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        RetryCount++;
        Error = error;
    }
}
