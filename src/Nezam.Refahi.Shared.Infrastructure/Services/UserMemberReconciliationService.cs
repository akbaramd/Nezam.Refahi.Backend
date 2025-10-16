using Nezam.Refahi.Shared.Domain.Entities;
using Nezam.Refahi.Shared.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Nezam.Refahi.Shared.Infrastructure.Services;

/// <summary>
/// Implementation of User-Member reconciliation service
/// </summary>
public class UserMemberReconciliationService : IUserMemberReconciliationService
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<UserMemberReconciliationService> _logger;

    public UserMemberReconciliationService(
        IOutboxRepository outboxRepository,
        ILogger<UserMemberReconciliationService> logger)
    {
        _outboxRepository = outboxRepository ?? throw new ArgumentNullException(nameof(outboxRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ReconciliationResult> ReconcileOrphanedUsersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting reconciliation of orphaned users");

        var result = new ReconciliationResult();
        var startTime = DateTime.UtcNow;

        try
        {
            // Get orphaned users from outbox messages
            var orphanedUserEvents = await GetOrphanedUserEventsAsync(cancellationToken);
            result.ProcessedCount = orphanedUserEvents.Count();

            foreach (var userEvent in orphanedUserEvents)
            {
                var action = new ReconciliationAction
                {
                    EntityId = userEvent.AggregateId,
                    EntityType = "User",
                    Description = $"Processing orphaned user event: {userEvent.Type}"
                };

                try
                {
                    // Attempt to republish the event
                    var republishSuccess = await RepublishUserEventAsync(userEvent, cancellationToken);
                    
                    if (republishSuccess)
                    {
                        action.ActionType = ReconciliationActionTypes.RepublishEvent;
                        action.Success = true;
                        result.FixedCount++;
                    }
                    else
                    {
                        action.ActionType = ReconciliationActionTypes.SkipProcessing;
                        action.Success = false;
                        action.ErrorMessage = "Failed to republish event";
                        result.SkippedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing orphaned user event {EventId}", userEvent.Id);
                    action.ActionType = ReconciliationActionTypes.SkipProcessing;
                    action.Success = false;
                    action.ErrorMessage = ex.Message;
                    result.FailedCount++;
                    result.Errors.Add($"Error processing user event {userEvent.Id}: {ex.Message}");
                }

                result.Actions.Add(action);
            }

            result.Duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Orphaned users reconciliation completed: {ProcessedCount} processed, {FixedCount} fixed, {FailedCount} failed, {SkippedCount} skipped",
                result.ProcessedCount, result.FixedCount, result.FailedCount, result.SkippedCount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during orphaned users reconciliation");
            result.Duration = DateTime.UtcNow - startTime;
            result.Errors.Add($"Reconciliation failed: {ex.Message}");
            throw;
        }
    }

    public async Task<ReconciliationResult> ReconcileOrphanedMembersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting reconciliation of orphaned members");

        var result = new ReconciliationResult();
        var startTime = DateTime.UtcNow;

        try
        {
            // Get orphaned members from outbox messages
            var orphanedMemberEvents = await GetOrphanedMemberEventsAsync(cancellationToken);
            result.ProcessedCount = orphanedMemberEvents.Count();

            foreach (var memberEvent in orphanedMemberEvents)
            {
                var action = new ReconciliationAction
                {
                    EntityId = memberEvent.AggregateId,
                    EntityType = "Member",
                    Description = $"Processing orphaned member event: {memberEvent.Type}"
                };

                try
                {
                    // Attempt to republish the event
                    var republishSuccess = await RepublishMemberEventAsync(memberEvent, cancellationToken);
                    
                    if (republishSuccess)
                    {
                        action.ActionType = ReconciliationActionTypes.RepublishEvent;
                        action.Success = true;
                        result.FixedCount++;
                    }
                    else
                    {
                        action.ActionType = ReconciliationActionTypes.SkipProcessing;
                        action.Success = false;
                        action.ErrorMessage = "Failed to republish event";
                        result.SkippedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing orphaned member event {EventId}", memberEvent.Id);
                    action.ActionType = ReconciliationActionTypes.SkipProcessing;
                    action.Success = false;
                    action.ErrorMessage = ex.Message;
                    result.FailedCount++;
                    result.Errors.Add($"Error processing member event {memberEvent.Id}: {ex.Message}");
                }

                result.Actions.Add(action);
            }

            result.Duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Orphaned members reconciliation completed: {ProcessedCount} processed, {FixedCount} fixed, {FailedCount} failed, {SkippedCount} skipped",
                result.ProcessedCount, result.FixedCount, result.FailedCount, result.SkippedCount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during orphaned members reconciliation");
            result.Duration = DateTime.UtcNow - startTime;
            result.Errors.Add($"Reconciliation failed: {ex.Message}");
            throw;
        }
    }

    public async Task<ReconciliationResult> RepairBrokenMappingsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting repair of broken User-Member mappings");

        var result = new ReconciliationResult();
        var startTime = DateTime.UtcNow;

        try
        {
            // Get broken mapping events
            var brokenMappingEvents = await GetBrokenMappingEventsAsync(cancellationToken);
            result.ProcessedCount = brokenMappingEvents.Count();

            foreach (var mappingEvent in brokenMappingEvents)
            {
                var action = new ReconciliationAction
                {
                    EntityId = mappingEvent.AggregateId,
                    EntityType = "UserMemberMapping",
                    Description = $"Repairing broken mapping: {mappingEvent.Type}"
                };

                try
                {
                    // Attempt to repair the mapping
                    var repairSuccess = await RepairMappingAsync(mappingEvent, cancellationToken);
                    
                    if (repairSuccess)
                    {
                        action.ActionType = ReconciliationActionTypes.LinkUserMember;
                        action.Success = true;
                        result.FixedCount++;
                    }
                    else
                    {
                        action.ActionType = ReconciliationActionTypes.SkipProcessing;
                        action.Success = false;
                        action.ErrorMessage = "Failed to repair mapping";
                        result.SkippedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error repairing mapping event {EventId}", mappingEvent.Id);
                    action.ActionType = ReconciliationActionTypes.SkipProcessing;
                    action.Success = false;
                    action.ErrorMessage = ex.Message;
                    result.FailedCount++;
                    result.Errors.Add($"Error repairing mapping event {mappingEvent.Id}: {ex.Message}");
                }

                result.Actions.Add(action);
            }

            result.Duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Broken mappings repair completed: {ProcessedCount} processed, {FixedCount} fixed, {FailedCount} failed, {SkippedCount} skipped",
                result.ProcessedCount, result.FixedCount, result.FailedCount, result.SkippedCount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during broken mappings repair");
            result.Duration = DateTime.UtcNow - startTime;
            result.Errors.Add($"Repair failed: {ex.Message}");
            throw;
        }
    }

    public async Task<ReconciliationResult> ProcessDlqMessagesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting DLQ messages processing");

        var result = new ReconciliationResult();
        var startTime = DateTime.UtcNow;

        try
        {
            // Get DLQ messages related to User-Member events
            var dlqMessages = await GetUserMemberDlqMessagesAsync(cancellationToken);
            result.ProcessedCount = dlqMessages.Count();

            foreach (var dlqMessage in dlqMessages)
            {
                var action = new ReconciliationAction
                {
                    EntityId = dlqMessage.AggregateId,
                    EntityType = "OutboxMessage",
                    Description = $"Processing DLQ message: {dlqMessage.Type}"
                };

                try
                {
                    // Determine action based on message characteristics
                    if (dlqMessage.IsPoisoned)
                    {
                        action.ActionType = ReconciliationActionTypes.MarkAsPoison;
                        action.Success = true;
                        result.SkippedCount++;
                    }
                    else
                    {
                        // Attempt to retry the message
                        var retrySuccess = await RetryDlqMessageAsync(dlqMessage, cancellationToken);
                        
                        if (retrySuccess)
                        {
                            action.ActionType = ReconciliationActionTypes.RepublishEvent;
                            action.Success = true;
                            result.FixedCount++;
                        }
                        else
                        {
                            action.ActionType = ReconciliationActionTypes.SkipProcessing;
                            action.Success = false;
                            action.ErrorMessage = "Failed to retry message";
                            result.SkippedCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing DLQ message {MessageId}", dlqMessage.Id);
                    action.ActionType = ReconciliationActionTypes.SkipProcessing;
                    action.Success = false;
                    action.ErrorMessage = ex.Message;
                    result.FailedCount++;
                    result.Errors.Add($"Error processing DLQ message {dlqMessage.Id}: {ex.Message}");
                }

                result.Actions.Add(action);
            }

            result.Duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("DLQ messages processing completed: {ProcessedCount} processed, {FixedCount} fixed, {FailedCount} failed, {SkippedCount} skipped",
                result.ProcessedCount, result.FixedCount, result.FailedCount, result.SkippedCount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DLQ messages processing");
            result.Duration = DateTime.UtcNow - startTime;
            result.Errors.Add($"DLQ processing failed: {ex.Message}");
            throw;
        }
    }

    public async Task<ComprehensiveReconciliationResult> PerformComprehensiveReconciliationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting comprehensive User-Member reconciliation");

        var startTime = DateTime.UtcNow;
        var result = new ComprehensiveReconciliationResult();

        try
        {
            // Run all reconciliation tasks in parallel
            var tasks = new[]
            {
                ReconcileOrphanedUsersAsync(cancellationToken),
                ReconcileOrphanedMembersAsync(cancellationToken),
                RepairBrokenMappingsAsync(cancellationToken),
                ProcessDlqMessagesAsync(cancellationToken)
            };

            var results = await Task.WhenAll(tasks);

            result.OrphanedUsers = results[0];
            result.OrphanedMembers = results[1];
            result.BrokenMappings = results[2];
            result.DlqMessages = results[3];
            result.TotalDuration = DateTime.UtcNow - startTime;

            _logger.LogInformation("Comprehensive reconciliation completed in {Duration}: {TotalProcessed} total processed, {TotalFixed} total fixed, {TotalFailed} total failed",
                result.TotalDuration, result.TotalProcessed, result.TotalFixed, result.TotalFailed);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during comprehensive reconciliation");
            result.TotalDuration = DateTime.UtcNow - startTime;
            throw;
        }
    }

    #region Private Helper Methods

    private async Task<IEnumerable<OutboxMessage>> GetOrphanedUserEventsAsync(CancellationToken cancellationToken)
    {
        // Get UserCreated events that haven't been processed successfully
        return await _outboxRepository.GetUnprocessedMessagesByTypeAsync("UserCreatedEvent", cancellationToken);
    }

    private async Task<IEnumerable<OutboxMessage>> GetOrphanedMemberEventsAsync(CancellationToken cancellationToken)
    {
        // Get MemberCreated events that haven't been processed successfully
        return await _outboxRepository.GetUnprocessedMessagesByTypeAsync("MemberCreatedEvent", cancellationToken);
    }

    private async Task<IEnumerable<OutboxMessage>> GetBrokenMappingEventsAsync(CancellationToken cancellationToken)
    {
        // Get UserMemberMapping events that failed
        return await _outboxRepository.GetFailedMessagesByTypeAsync("UserMemberMappingEvent", cancellationToken);
    }

    private async Task<IEnumerable<OutboxMessage>> GetUserMemberDlqMessagesAsync(CancellationToken cancellationToken)
    {
        // Get DLQ messages related to User-Member events
        var userMemberTypes = new[] { "UserCreatedEvent", "MemberCreatedEvent", "UserMemberMappingEvent" };
        var dlqMessages = new List<OutboxMessage>();

        foreach (var type in userMemberTypes)
        {
            var messages = await _outboxRepository.GetDlqMessagesByTypeAsync(type, cancellationToken);
            dlqMessages.AddRange(messages);
        }

        return dlqMessages;
    }

    private async Task<bool> RepublishUserEventAsync(OutboxMessage userEvent, CancellationToken cancellationToken)
    {
        try
        {
            // Reset the message for retry
            userEvent.ResetForRetry();
            await _outboxRepository.UpdateAsync(userEvent);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to republish user event {EventId}", userEvent.Id);
            return false;
        }
    }

    private async Task<bool> RepublishMemberEventAsync(OutboxMessage memberEvent, CancellationToken cancellationToken)
    {
        try
        {
            // Reset the message for retry
            memberEvent.ResetForRetry();
            await _outboxRepository.UpdateAsync(memberEvent);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to republish member event {EventId}", memberEvent.Id);
            return false;
        }
    }

    private async Task<bool> RepairMappingAsync(OutboxMessage mappingEvent, CancellationToken cancellationToken)
    {
        try
        {
            // Reset the message for retry
            mappingEvent.ResetForRetry();
            await _outboxRepository.UpdateAsync(mappingEvent);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to repair mapping event {EventId}", mappingEvent.Id);
            return false;
        }
    }

    private async Task<bool> RetryDlqMessageAsync(OutboxMessage dlqMessage, CancellationToken cancellationToken)
    {
        try
        {
            // Reset the message for retry
            dlqMessage.ResetForRetry();
            await _outboxRepository.UpdateAsync(dlqMessage);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retry DLQ message {MessageId}", dlqMessage.Id);
            return false;
        }
    }

    #endregion
}
