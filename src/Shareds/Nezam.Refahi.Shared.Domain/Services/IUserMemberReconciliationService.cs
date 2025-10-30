using Nezam.Refahi.Shared.Domain.Entities;

namespace Nezam.Refahi.Shared.Domain.Services;

/// <summary>
/// Service for reconciling User-Member mappings and handling orphaned entities
/// </summary>
public interface IUserMemberReconciliationService
{
    /// <summary>
    /// Reconciles orphaned users (users without corresponding members)
    /// </summary>
    Task<ReconciliationResult> ReconcileOrphanedUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reconciles orphaned members (members without corresponding users)
    /// </summary>
    Task<ReconciliationResult> ReconcileOrphanedMembersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Repairs broken User-Member mappings
    /// </summary>
    Task<ReconciliationResult> RepairBrokenMappingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes DLQ messages for User-Member events
    /// </summary>
    Task<ReconciliationResult> ProcessDlqMessagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs comprehensive reconciliation of all User-Member issues
    /// </summary>
    Task<ComprehensiveReconciliationResult> PerformComprehensiveReconciliationAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a reconciliation operation
/// </summary>
public class ReconciliationResult
{
    public int ProcessedCount { get; set; }
    public int FixedCount { get; set; }
    public int FailedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<ReconciliationAction> Actions { get; set; } = new();
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Comprehensive reconciliation result
/// </summary>
public class ComprehensiveReconciliationResult
{
    public ReconciliationResult OrphanedUsers { get; set; } = new();
    public ReconciliationResult OrphanedMembers { get; set; } = new();
    public ReconciliationResult BrokenMappings { get; set; } = new();
    public ReconciliationResult DlqMessages { get; set; } = new();
    public int TotalProcessed => OrphanedUsers.ProcessedCount + OrphanedMembers.ProcessedCount + 
                                 BrokenMappings.ProcessedCount + DlqMessages.ProcessedCount;
    public int TotalFixed => OrphanedUsers.FixedCount + OrphanedMembers.FixedCount + 
                            BrokenMappings.FixedCount + DlqMessages.FixedCount;
    public int TotalFailed => OrphanedUsers.FailedCount + OrphanedMembers.FailedCount + 
                             BrokenMappings.FailedCount + DlqMessages.FailedCount;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan TotalDuration { get; set; }
}

/// <summary>
/// Represents a reconciliation action taken
/// </summary>
public class ReconciliationAction
{
    public string ActionType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Types of reconciliation actions
/// </summary>
public static class ReconciliationActionTypes
{
    public const string CreateMember = "CREATE_MEMBER";
    public const string DeleteMember = "DELETE_MEMBER";
    public const string LinkUserMember = "LINK_USER_MEMBER";
    public const string UnlinkUserMember = "UNLINK_USER_MEMBER";
    public const string RepublishEvent = "REPUBLISH_EVENT";
    public const string MarkAsPoison = "MARK_AS_POISON";
    public const string SkipProcessing = "SKIP_PROCESSING";
}
