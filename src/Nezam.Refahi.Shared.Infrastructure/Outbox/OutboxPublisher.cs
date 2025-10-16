using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Domain.Entities;
using Nezam.Refahi.Shared.Domain.Repositories;
using Nezam.Refahi.Shared.Infrastructure.Services;

namespace Nezam.Refahi.Shared.Infrastructure.Outbox;

/// <summary>
/// Implementation of outbox publisher for reliable event publishing
/// Enhanced with idempotency and correlation support
/// </summary>
public class OutboxPublisher : IOutboxPublisher
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(
        IOutboxRepository outboxRepository,
        ILogger<OutboxPublisher> logger)
    {
        _outboxRepository = outboxRepository ?? throw new ArgumentNullException(nameof(outboxRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            await _outboxRepository.PublishMessageAsync(integrationEvent, cancellationToken);
            
            _logger.LogDebug("Published integration event {EventType} to outbox", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish integration event {EventType} to outbox", typeof(T).Name);
            throw;
        }
    }

    public async Task PublishAsync<T>(T integrationEvent, Guid? aggregateId = null, string? correlationId = null, string? idempotencyKey = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var message = OutboxMessage.Create(
                integrationEvent,
                aggregateId: aggregateId,
                correlationId: correlationId,
                idempotencyKey: idempotencyKey,
                schemaVersion: 1
            );

            // Ensure payload content is serialized JSON with proper dictionary handling
            message.Content = JsonSerializationService.SerializeCamelCase(integrationEvent);

            await _outboxRepository.AddAsync(message, cancellationToken);
            
            _logger.LogDebug("Published integration event {EventType} to outbox with aggregate {AggregateId}, correlation {CorrelationId}, idempotency {IdempotencyKey}", 
                typeof(T).Name, aggregateId, correlationId, idempotencyKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish integration event {EventType} to outbox with enhanced metadata", typeof(T).Name);
            throw;
        }
    }

    public async Task PublishAsync<T>(IEnumerable<T> integrationEvents, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            await _outboxRepository.PublishMessagesAsync(integrationEvents, cancellationToken);
            
            _logger.LogDebug("Published {Count} integration events of type {EventType} to outbox", 
                integrationEvents.Count(), typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish integration events of type {EventType} to outbox", typeof(T).Name);
            throw;
        }
    }

    public async Task PublishAsync<T>(IEnumerable<T> integrationEvents, Guid? aggregateId = null, string? correlationId = null, string? idempotencyKey = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var messages = integrationEvents.Select(integrationEvent => OutboxMessage.Create(
                integrationEvent,
                aggregateId: aggregateId,
                correlationId: correlationId,
                idempotencyKey: idempotencyKey,
                schemaVersion: 1
            )).ToList();

            // Serialize content for each message with proper dictionary handling
            int i = 0;
            foreach (var integrationEvent in integrationEvents)
            {
                messages[i].Content = JsonSerializationService.SerializeCamelCase(integrationEvent);
                i++;
            }

            await _outboxRepository.AddRangeAsync(messages, cancellationToken);
            
            _logger.LogDebug("Published {Count} integration events of type {EventType} to outbox with enhanced metadata", 
                integrationEvents.Count(), typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish integration events of type {EventType} to outbox with enhanced metadata", typeof(T).Name);
            throw;
        }
    }
}
