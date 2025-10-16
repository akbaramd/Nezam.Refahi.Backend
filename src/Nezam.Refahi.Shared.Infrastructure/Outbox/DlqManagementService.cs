using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Domain.Entities;
using Nezam.Refahi.Shared.Domain.Repositories;

namespace Nezam.Refahi.Shared.Infrastructure.Outbox;

/// <summary>
/// Service for managing Dead Letter Queue (DLQ) operations
/// </summary>
public interface IDlqManagementService
{
    /// <summary>
    /// Processes messages in DLQ for manual intervention
    /// </summary>
    Task<DlqProcessingResult> ProcessDlqMessagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retries a specific DLQ message
    /// </summary>
    Task<bool> RetryDlqMessageAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a DLQ message as permanently failed
    /// </summary>
    Task<bool> MarkDlqMessageAsPermanentlyFailedAsync(Guid messageId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics about DLQ messages
    /// </summary>
    Task<DlqStatistics> GetDlqStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up old DLQ messages
    /// </summary>
    Task<int> CleanupOldDlqMessagesAsync(int retentionDays = 30, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of DLQ processing operation
/// </summary>
public class DlqProcessingResult
{
    public int TotalMessages { get; set; }
    public int RetriedMessages { get; set; }
    public int PermanentlyFailedMessages { get; set; }
    public int SkippedMessages { get; set; }
    public List<DlqMessageAction> Actions { get; set; } = new();
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Statistics about DLQ messages
/// </summary>
public class DlqStatistics
{
    public int TotalDlqMessages { get; set; }
    public int PoisonMessages { get; set; }
    public int MaxRetriesExceededMessages { get; set; }
    public int OldestMessageAgeHours { get; set; }
    public Dictionary<string, int> MessagesByType { get; set; } = new();
    public Dictionary<string, int> MessagesByReason { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents an action taken on a DLQ message
/// </summary>
public class DlqMessageAction
{
    public Guid MessageId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Implementation of DLQ management service
/// </summary>
public class DlqManagementService : IDlqManagementService
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<DlqManagementService> _logger;

    public DlqManagementService(
        IOutboxRepository outboxRepository,
        ILogger<DlqManagementService> logger)
    {
        _outboxRepository = outboxRepository ?? throw new ArgumentNullException(nameof(outboxRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DlqProcessingResult> ProcessDlqMessagesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting DLQ message processing");

        var result = new DlqProcessingResult();
        
        try
        {
            // Get all DLQ messages
            var dlqMessages = await _outboxRepository.GetDlqMessagesAsync(cancellationToken);
            result.TotalMessages = dlqMessages.Count();

            foreach (var message in dlqMessages)
            {
                var action = new DlqMessageAction
                {
                    MessageId = message.Id,
                    MessageType = message.Type,
                    Reason = message.DlqReason ?? "Unknown"
                };

                try
                {
                    // Determine action based on message characteristics
                    if (message.IsPoisoned)
                    {
                        // Poison messages should be marked as permanently failed
                        await MarkDlqMessageAsPermanentlyFailedAsync(message.Id, "Poison message", cancellationToken);
                        action.ActionType = "PERMANENTLY_FAILED";
                        action.Success = true;
                        result.PermanentlyFailedMessages++;
                    }
                    else if (message.RetryCount < message.MaxRetries)
                    {
                        // Messages that haven't exceeded max retries can be retried
                        var retrySuccess = await RetryDlqMessageAsync(message.Id, cancellationToken);
                        action.ActionType = "RETRY";
                        action.Success = retrySuccess;
                        if (retrySuccess)
                        {
                            result.RetriedMessages++;
                        }
                        else
                        {
                            result.SkippedMessages++;
                        }
                    }
                    else
                    {
                        // Messages that exceeded max retries should be marked as permanently failed
                        await MarkDlqMessageAsPermanentlyFailedAsync(message.Id, "Max retries exceeded", cancellationToken);
                        action.ActionType = "PERMANENTLY_FAILED";
                        action.Success = true;
                        result.PermanentlyFailedMessages++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing DLQ message {MessageId}", message.Id);
                    action.ActionType = "ERROR";
                    action.Success = false;
                    action.ErrorMessage = ex.Message;
                    result.SkippedMessages++;
                }

                result.Actions.Add(action);
            }

            _logger.LogInformation("DLQ processing completed: {TotalMessages} total, {RetriedMessages} retried, {PermanentlyFailedMessages} permanently failed, {SkippedMessages} skipped",
                result.TotalMessages, result.RetriedMessages, result.PermanentlyFailedMessages, result.SkippedMessages);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DLQ processing");
            throw;
        }
    }

    public async Task<bool> RetryDlqMessageAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = await _outboxRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                _logger.LogWarning("DLQ message {MessageId} not found", messageId);
                return false;
            }

            if (!message.IsInDlq)
            {
                _logger.LogWarning("Message {MessageId} is not in DLQ", messageId);
                return false;
            }

            // Reset the message for retry
            message.ResetForRetry();
            await _outboxRepository.UpdateAsync(message);

            _logger.LogInformation("DLQ message {MessageId} reset for retry", messageId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrying DLQ message {MessageId}", messageId);
            return false;
        }
    }

    public async Task<bool> MarkDlqMessageAsPermanentlyFailedAsync(Guid messageId, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = await _outboxRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                _logger.LogWarning("DLQ message {MessageId} not found", messageId);
                return false;
            }

            // Mark as permanently failed
            message.IsPoisonMessage = true;
            message.PoisonedAt = DateTime.UtcNow;
            message.FailureReason = reason;
            await _outboxRepository.UpdateAsync(message);

            _logger.LogInformation("DLQ message {MessageId} marked as permanently failed: {Reason}", messageId, reason);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking DLQ message {MessageId} as permanently failed", messageId);
            return false;
        }
    }

    public async Task<DlqStatistics> GetDlqStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var dlqMessages = await _outboxRepository.GetDlqMessagesAsync(cancellationToken);
            var messages = dlqMessages.ToList();

            var statistics = new DlqStatistics
            {
                TotalDlqMessages = messages.Count,
                PoisonMessages = messages.Count(m => m.IsPoisoned),
                MaxRetriesExceededMessages = messages.Count(m => m.RetryCount >= m.MaxRetries),
                MessagesByType = messages.GroupBy(m => m.Type)
                    .ToDictionary(g => g.Key, g => g.Count()),
                MessagesByReason = messages.GroupBy(m => m.DlqReason ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            if (messages.Any())
            {
                var oldestMessage = messages.MinBy(m => m.OccurredOn);
                if (oldestMessage != null)
                {
                    statistics.OldestMessageAgeHours = (int)(DateTime.UtcNow - oldestMessage.OccurredOn).TotalHours;
                }
            }

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DLQ statistics");
            throw;
        }
    }

    public async Task<int> CleanupOldDlqMessagesAsync(int retentionDays = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
            var oldDlqMessages = await _outboxRepository.GetDlqMessagesOlderThanAsync(cutoffDate, cancellationToken);
            
            var messageIds = oldDlqMessages.Select(m => m.Id).ToList();
            if (messageIds.Any())
            {
                await _outboxRepository.DeleteMessagesAsync(messageIds, cancellationToken);
                _logger.LogInformation("Cleaned up {Count} old DLQ messages older than {CutoffDate}", 
                    messageIds.Count, cutoffDate);
            }

            return messageIds.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old DLQ messages");
            throw;
        }
    }
}
