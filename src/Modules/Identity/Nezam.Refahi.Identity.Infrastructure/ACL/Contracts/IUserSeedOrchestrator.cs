using System.Threading;
using System.Threading.Tasks;
using Nezam.Refahi.Identity.Infrastructure.ACL.Contracts;

namespace Nezam.Refahi.Identity.Application.Services.Contracts;

/// <summary>
/// Application service for orchestrating user seeding operations
/// Provides high-level coordination of seeding processes with proper error handling and metrics
/// </summary>
public interface IUserSeedOrchestrator
{
    /// <summary>
    /// Runs seeding operation once with specified parameters
    /// </summary>
    /// <param name="batchSize">Number of users to process per batch</param>
    /// <param name="maxParallel">Maximum number of parallel batches</param>
    /// <param name="source">Source system to seed from</param>
    /// <param name="dryRun">If true, validates but doesn't create users</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the seeding operation</returns>
    Task<UserSeedResult> RunOnceAsync(
        int batchSize = 1000,
        int maxParallel = 4,
        IUserSeedSource? source = null,
        bool dryRun = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs incremental seeding from watermark
    /// </summary>
    /// <param name="watermark">Watermark to start from</param>
    /// <param name="batchSize">Number of users to process per batch</param>
    /// <param name="maxParallel">Maximum number of parallel batches</param>
    /// <param name="source">Source system to seed from</param>
    /// <param name="dryRun">If true, validates but doesn't create users</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the seeding operation</returns>
    Task<UserSeedResult> RunIncrementalAsync(
        UserSeedWatermark watermark,
        int batchSize = 1000,
        int maxParallel = 4,
        IUserSeedSource? source = null,
        bool dryRun = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets seeding statistics and metrics
    /// </summary>
    Task<UserSeedStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates seeding configuration and source availability
    /// </summary>
    Task<UserSeedValidationResult> ValidateConfigurationAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a user seeding operation
/// </summary>
public class UserSeedResult
{
    public int TotalProcessed { get; set; }
    public int SuccessfullyCreated { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
    public int ValidationErrors { get; set; }
    public List<UserSeedError> Errors { get; set; } = new();
    public List<UserSeedWarning> Warnings { get; set; } = new();
    public UserSeedWatermark? LastWatermark { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public bool IsDryRun { get; set; }
    public string SourceSystem { get; set; } = string.Empty;
}

/// <summary>
/// Statistics about user seeding operations
/// </summary>
public class UserSeedStatistics
{
    public int TotalUsersSeeded { get; set; }
    public int TotalBatchesProcessed { get; set; }
    public int TotalErrors { get; set; }
    public int TotalSkipped { get; set; }
    public Dictionary<string, int> UsersBySource { get; set; } = new();
    public Dictionary<string, int> ErrorsByType { get; set; } = new();
    public TimeSpan AverageBatchDuration { get; set; }
    public DateTime LastSeedingRun { get; set; }
    public UserSeedWatermark? LastWatermark { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Error information from seeding operation
/// </summary>
public class UserSeedError
{
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ExternalUserId { get; set; }
    public string? SourceSystem { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Warning information from seeding operation
/// </summary>
public class UserSeedWarning
{
    public string WarningCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ExternalUserId { get; set; }
    public string? SourceSystem { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
