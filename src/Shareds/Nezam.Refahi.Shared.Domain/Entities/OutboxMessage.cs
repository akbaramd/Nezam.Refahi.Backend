using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Shared.Domain.Entities;

/// <summary>
/// Represents an outbox message for reliable event publishing with comprehensive failure handling
/// </summary>
public class OutboxMessage : Entity<Guid>
{
    // Basic event information
    public string Type { get; set; } = string.Empty;
    public string FullTypeName { get; set; } = string.Empty;
    public string AssemblyName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    
    // Processing status
    public DateTime? ProcessedOn { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; } = 0;
    public int MaxRetries { get; set; } = 3;
    
    // Enhanced failure handling
    public string? IdempotencyKey { get; set; }
    public Guid? AggregateId { get; set; }
    public string? CorrelationId { get; set; }
    public int SchemaVersion { get; set; } = 1;
    public DateTime? NextRetryAt { get; set; }
    public string? FailureReason { get; set; }
    public bool IsPoisonMessage { get; set; } = false;
    public DateTime? PoisonedAt { get; set; }
    
    // DLQ management
    public DateTime? MovedToDlqAt { get; set; }
    public string? DlqReason { get; set; }
    
    // Computed properties
    public bool IsProcessed => ProcessedOn.HasValue;
    public bool IsFailed => RetryCount >= MaxRetries && !IsProcessed;
    public bool IsPoisoned => IsPoisonMessage;
    public bool IsInDlq => MovedToDlqAt.HasValue;
    public bool ShouldRetry => !IsProcessed && !IsFailed && !IsPoisoned && 
                               (NextRetryAt == null || DateTime.UtcNow >= NextRetryAt);

    public OutboxMessage() : base(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Creates a new outbox message with enhanced metadata
    /// </summary>
    public static OutboxMessage Create<T>(
        T integrationEvent,
        Guid? aggregateId = null,
        string? correlationId = null,
        string? idempotencyKey = null,
        int schemaVersion = 1)
    {
        var message = new OutboxMessage
        {
            Type = typeof(T).Name,
            FullTypeName = typeof(T).FullName ?? typeof(T).Name,
            AssemblyName = typeof(T).Assembly.GetName().Name ?? "Unknown",
            AggregateId = aggregateId,
            CorrelationId = correlationId,
            IdempotencyKey = idempotencyKey,
            SchemaVersion = schemaVersion,
            OccurredOn = DateTime.UtcNow
        };

        return message;
    }

    /// <summary>
    /// Marks the message as processed
    /// </summary>
    public void MarkAsProcessed()
    {
        ProcessedOn = DateTime.UtcNow;
        Error = null;
        FailureReason = null;
    }

    /// <summary>
    /// Marks the message as failed with retry scheduling
    /// </summary>
    public void MarkAsFailed(string error, bool isPoisonMessage = false)
    {
        RetryCount++;
        Error = error;
        FailureReason = error;
        
        if (isPoisonMessage)
        {
            IsPoisonMessage = true;
            PoisonedAt = DateTime.UtcNow;
            MovedToDlqAt = DateTime.UtcNow;
            DlqReason = "Poison message detected";
        }
        else if (RetryCount >= MaxRetries)
        {
            MovedToDlqAt = DateTime.UtcNow;
            DlqReason = "Max retries exceeded";
        }
        else
        {
            // Schedule next retry with exponential backoff
            var delayMinutes = Math.Min(Math.Pow(2, RetryCount), 60); // Max 60 minutes
            NextRetryAt = DateTime.UtcNow.AddMinutes(delayMinutes);
        }
    }

    /// <summary>
    /// Resets retry count for manual retry
    /// </summary>
    public void ResetForRetry()
    {
        RetryCount = 0;
        Error = null;
        FailureReason = null;
        NextRetryAt = null;
        IsPoisonMessage = false;
        PoisonedAt = null;
        MovedToDlqAt = null;
        DlqReason = null;
    }
}
