using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hangfire;
using Nezam.Refahi.Shared.Domain.Entities;
using Nezam.Refahi.Shared.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.Services;
using Nezam.Refahi.Shared.Infrastructure.Services;

namespace Nezam.Refahi.Shared.Infrastructure.Outbox;

/// <summary>
/// Hangfire-based outbox message processor
/// </summary>
public class OutboxProcessor
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IIdempotencyService _idempotencyService;

    public OutboxProcessor(
        IOutboxRepository outboxRepository,
        IMediator mediator,
        ILogger<OutboxProcessor> logger,
        IIdempotencyService idempotencyService)
    {
        _outboxRepository = outboxRepository ?? throw new ArgumentNullException(nameof(outboxRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _idempotencyService = idempotencyService ?? throw new ArgumentNullException(nameof(idempotencyService));
    }



    /// <summary>
    /// Processes outbox messages continuously using a while loop
    /// This method runs in a single background job and processes messages continuously
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessOutboxMessagesContinuouslyAsync()
    {
        _logger.LogInformation("Starting continuous outbox message processing");

        int consecutiveErrors = 0;
        const int maxConsecutiveErrors = 5;
        const int baseDelaySeconds = 5;
        const int maxDelaySeconds = 60;

        while (true)
        {
            try
            {
                _logger.LogDebug("Processing outbox messages batch");

                var unprocessedMessages = await _outboxRepository.GetUnprocessedMessagesAsync(50);

                if (unprocessedMessages.Any())
                {
                    _logger.LogDebug("Found {Count} unprocessed messages", unprocessedMessages.Count());

                    // Process messages sequentially to avoid shared DbContext concurrency
                    foreach (var message in unprocessedMessages)
                    {
                        try
                        {
                            await ProcessMessageInternalAsync(message, _mediator, _outboxRepository);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                            await _outboxRepository.MarkAsFailedAsync(message.Id, ex.Message);
                        }
                    }
                    _logger.LogDebug("Processed {Count} outbox messages", unprocessedMessages.Count());
                    
                    // Reset error counter on successful processing
                    consecutiveErrors = 0;
                }
                else
                {
                    _logger.LogDebug("No unprocessed messages found");
                    // Reset error counter when no messages to process
                    consecutiveErrors = 0;
                }

                // Calculate delay based on consecutive errors (exponential backoff)
                var delaySeconds = Math.Min(baseDelaySeconds * Math.Pow(2, consecutiveErrors), maxDelaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
            catch (Exception ex)
            {
                consecutiveErrors++;
                _logger.LogError(ex, "Error during outbox processing (consecutive errors: {ConsecutiveErrors}), retrying in {DelaySeconds} seconds", 
                    consecutiveErrors, Math.Min(baseDelaySeconds * Math.Pow(2, consecutiveErrors), maxDelaySeconds));

                // If too many consecutive errors, wait longer
                if (consecutiveErrors >= maxConsecutiveErrors)
                {
                    _logger.LogWarning("Too many consecutive errors ({ConsecutiveErrors}), waiting {MaxDelay} seconds before retry", 
                        consecutiveErrors, maxDelaySeconds);
                    await Task.Delay(TimeSpan.FromSeconds(maxDelaySeconds));
                    consecutiveErrors = 0; // Reset after long wait
                }
                else
                {
                    var delaySeconds = Math.Min(baseDelaySeconds * Math.Pow(2, consecutiveErrors), maxDelaySeconds);
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }
        }
    }

    /// <summary>
    /// Processes outbox messages and schedules the next processing job
    /// This method processes a batch and then schedules itself to run again
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessOutboxMessagesAsync()
    {
        _logger.LogDebug("Processing outbox messages batch");

        try
        {
            var unprocessedMessages = await _outboxRepository.GetUnprocessedMessagesAsync(50);

            if (unprocessedMessages.Any())
            {
                _logger.LogDebug("Found {Count} unprocessed messages", unprocessedMessages.Count());

                // Process sequentially to prevent DbContext concurrent operations
                foreach (var message in unprocessedMessages)
                {
                    try
                    {
                        await ProcessMessageInternalAsync(message, _mediator, _outboxRepository);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                        await _outboxRepository.MarkAsFailedAsync(message.Id, ex.Message);
                    }
                }
                _logger.LogDebug("Processed {Count} outbox messages", unprocessedMessages.Count());
            }
            else
            {
                _logger.LogDebug("No unprocessed messages found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during outbox processing");
            throw; // Let Hangfire handle retry
        }

        _logger.LogDebug("Completed outbox processing batch");
        
   
    }

    /// <summary>
    /// Processes a single outbox message
    /// This method is called by Hangfire for individual message processing
    /// </summary>
    [AutomaticRetry(Attempts = 5)]
    public async Task ProcessMessageAsync(Guid messageId)
    {
        _logger.LogDebug("Processing outbox message {MessageId}", messageId);

        var message = await _outboxRepository.GetByIdAsync(messageId);
        if (message == null)
        {
            _logger.LogWarning("Outbox message {MessageId} not found", messageId);
            return;
        }

        if (message.ProcessedOn.HasValue)
        {
            _logger.LogDebug("Outbox message {MessageId} already processed", messageId);
            return;
        }

        try
        {
            await ProcessMessageInternalAsync(message, _mediator, _outboxRepository);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process outbox message {MessageId}", messageId);
            await _outboxRepository.MarkAsFailedAsync(messageId, ex.Message);
            throw; // Let Hangfire handle retry logic
        }
    }

    private async Task ProcessMessageInternalAsync(
        OutboxMessage message, 
        IMediator mediator, 
        IOutboxRepository outboxRepository)
    {
        _logger.LogDebug("Processing outbox message {MessageId} of type {MessageType} (FullName: {FullTypeName})", 
            message.Id, message.Type, message.FullTypeName);

        try
        {
            // Check for poison message
            if (message.IsPoisoned)
            {
                _logger.LogWarning("Skipping poison message {MessageId}", message.Id);
                return;
            }

            // Skip idempotency check for outbox messages - they are already idempotent by design
            // The outbox pattern ensures messages are processed only once
            _logger.LogDebug("Processing outbox message {MessageId} (idempotency key: {IdempotencyKey}) - skipping idempotency check", 
                message.Id, message.IdempotencyKey ?? "none");

            // Deserialize the integration event using full type information
            var eventType = GetEventType(message.FullTypeName, message.AssemblyName);
            if (eventType == null)
            {
                var error = $"Unknown event type: {message.FullTypeName}";
                _logger.LogWarning("Unknown event type {EventType} (FullName: {FullTypeName}, Assembly: {AssemblyName}) for message {MessageId}", 
                    message.Type, message.FullTypeName, message.AssemblyName, message.Id);
                
                message.MarkAsFailed(error, isPoisonMessage: true);
                await outboxRepository.UpdateAsync(message);
                return;
            }

            // Guard against empty/invalid content
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                var error = "Empty outbox content";
                _logger.LogWarning("Outbox message {MessageId} has empty content. Marking as poison and moving to DLQ", message.Id);
                message.MarkAsFailed(error, isPoisonMessage: true);
                await outboxRepository.UpdateAsync(message);
                return;
            }

            var integrationEvent = JsonSerializationService.Deserialize(message.Content, eventType);

            if (integrationEvent == null)
            {
                var error = "Failed to deserialize event";
                _logger.LogWarning("Failed to deserialize message {MessageId} of type {EventType} (FullName: {FullTypeName})", 
                    message.Id, message.Type, message.FullTypeName);
                
                message.MarkAsFailed(error, isPoisonMessage: true);
                await outboxRepository.UpdateAsync(message);
                return;
            }

            // Publish the integration event through MediatR
            // This will trigger all registered INotificationHandler<T> implementations
            _logger.LogInformation("Publishing integration event {EventType} for message {MessageId} via MediatR", 
                eventType.Name, message.Id);
            await mediator.Publish(integrationEvent);
            _logger.LogInformation("Successfully published integration event {EventType} for message {MessageId}", 
                eventType.Name, message.Id);

            // Mark as processed
            message.MarkAsProcessed();
            await outboxRepository.UpdateAsync(message);

            // Mark idempotency if key exists
            if (!string.IsNullOrEmpty(message.IdempotencyKey))
            {
                try
                {
                    await _idempotencyService.MarkEventAsProcessedAsync(message.IdempotencyKey, message.AggregateId);
                    _logger.LogDebug("Marked event as processed for idempotency key {IdempotencyKey}", message.IdempotencyKey);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to mark event as processed for idempotency key {IdempotencyKey}", message.IdempotencyKey);
                    // Don't fail the entire processing if idempotency marking fails
                }
            }

            _logger.LogDebug("Successfully processed outbox message {MessageId}", message.Id);
        }
        catch (Exception ex)
        {
            var error = ex.Message;
            _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
            
            // Determine if this is a poison message
            var isPoisonMessage = IsPoisonMessage(ex);
            
            message.MarkAsFailed(error, isPoisonMessage);
            await outboxRepository.UpdateAsync(message);
            
            throw; // Re-throw to let Hangfire handle retry logic
        }
    }

    /// <summary>
    /// Determines if an exception represents a poison message
    /// </summary>
    private static bool IsPoisonMessage(Exception ex)
    {
        return ex switch
        {
            JsonException => true, // Schema/deserialization issues
            ArgumentException => true, // Invalid arguments
            InvalidOperationException => true, // Invalid state
            _ => false // Other exceptions might be transient
        };
    }

    private Type? GetEventType(string fullTypeName, string assemblyName)
    {
        try
        {
            // First try to get the type directly using the full type name
            var type = Type.GetType(fullTypeName);
            if (type != null)
                return type;

            // If that fails, try to load from the specific assembly
            if (!string.IsNullOrEmpty(assemblyName))
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == assemblyName);
                
                if (assembly != null)
                {
                    type = assembly.GetType(fullTypeName);
                    if (type != null)
                        return type;
                }
            }

            // Fallback: search in all Nezam.Refahi assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName?.Contains("Nezam.Refahi") == true);

            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(fullTypeName);
                if (type != null)
                    return type;
            }

            _logger.LogWarning("Could not resolve type {FullTypeName} from assembly {AssemblyName}", 
                fullTypeName, assemblyName);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving type {FullTypeName} from assembly {AssemblyName}", 
                fullTypeName, assemblyName);
            return null;
        }
    }
}
