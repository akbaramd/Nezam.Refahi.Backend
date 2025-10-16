using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nezam.Refahi.Shared.Domain.Repositories;

namespace Nezam.Refahi.WebApi.HealthChecks;

/// <summary>
/// Health check for outbox message processing
/// </summary>
public class OutboxHealthCheck : IHealthCheck
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<OutboxHealthCheck> _logger;

    public OutboxHealthCheck(IOutboxRepository outboxRepository, ILogger<OutboxHealthCheck> logger)
    {
        _outboxRepository = outboxRepository ?? throw new ArgumentNullException(nameof(outboxRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check for unprocessed messages
            var unprocessedMessages = await _outboxRepository.GetUnprocessedMessagesAsync(1, cancellationToken);
            var unprocessedCount = unprocessedMessages.Count();

            // Check for failed messages
            var failedMessages = await _outboxRepository.GetFailedMessagesAsync(1, cancellationToken);
            var failedCount = failedMessages.Count();

            var data = new Dictionary<string, object>
            {
                { "unprocessed_messages", unprocessedCount },
                { "failed_messages", failedCount }
            };

            // If there are too many failed messages, consider it unhealthy
            if (failedCount > 100)
            {
                return HealthCheckResult.Degraded($"Too many failed outbox messages: {failedCount}", data: data);
            }

            // If there are unprocessed messages, it's healthy but might be busy
            if (unprocessedCount > 0)
            {
                return HealthCheckResult.Healthy($"Outbox is processing {unprocessedCount} messages", data: data);
            }

            return HealthCheckResult.Healthy("Outbox is healthy", data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Outbox health check failed");
            return HealthCheckResult.Unhealthy("Outbox health check failed", ex);
        }
    }
}
