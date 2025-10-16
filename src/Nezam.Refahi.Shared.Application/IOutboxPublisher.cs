namespace Nezam.Refahi.Shared.Application;

/// <summary>
/// Service for publishing integration events through the outbox pattern
/// Enhanced with idempotency and correlation support
/// </summary>
public interface IOutboxPublisher
{
    /// <summary>
    /// Publishes an integration event through the outbox
    /// </summary>
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes an integration event with enhanced metadata
    /// </summary>
    Task PublishAsync<T>(T integrationEvent, Guid? aggregateId = null, string? correlationId = null, string? idempotencyKey = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes multiple integration events through the outbox
    /// </summary>
    Task PublishAsync<T>(IEnumerable<T> integrationEvents, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes multiple integration events with enhanced metadata
    /// </summary>
    Task PublishAsync<T>(IEnumerable<T> integrationEvents, Guid? aggregateId = null, string? correlationId = null, string? idempotencyKey = null, CancellationToken cancellationToken = default) where T : class;
}
