using System.Threading;
using System.Threading.Tasks;

namespace Nezam.Refahi.Identity.Infrastructure.ACL.Contracts;

/// <summary>
/// Anti-Corruption Layer contract for user seed sources
/// Provides abstraction over external systems to maintain bounded context isolation
/// </summary>
public interface IUserSeedSource
{
    /// <summary>
    /// Gets the name of the source system
    /// </summary>
    string SourceName { get; }

    /// <summary>
    /// Gets the version of the source system
    /// </summary>
    string SourceVersion { get; }

    /// <summary>
    /// Checks if the source is available and accessible
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a batch of user records from the source
    /// </summary>
    /// <param name="batchSize">Maximum number of records to return</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch of user seed records</returns>
    Task<UserSeedBatch> GetUserBatchAsync(int batchSize, int offset = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user records by watermark for incremental processing
    /// </summary>
    /// <param name="watermark">Watermark for incremental processing</param>
    /// <param name="batchSize">Maximum number of records to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch of user seed records</returns>
    Task<UserSeedBatch> GetUserBatchByWatermarkAsync(UserSeedWatermark watermark, int batchSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of users in the source (if available)
    /// </summary>
    Task<long?> GetTotalUserCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a user seed record
    /// </summary>
    Task<UserSeedValidationResult> ValidateUserRecordAsync(UserSeedRecord record, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a batch of user seed records
/// </summary>
public class UserSeedBatch
{
    public List<UserSeedRecord> Records { get; set; } = new();
    public bool HasMore { get; set; }
    public UserSeedWatermark? NextWatermark { get; set; }
    public int TotalInBatch { get; set; }
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents a user seed record from external sources
/// </summary>
public class UserSeedRecord
{
    // Core identity fields
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    
    // Optional fields
    public string? Email { get; set; }
    public string? Username { get; set; }
    
    // External source tracking
    public Guid ExternalUserId { get; set; }
    public string SourceSystem { get; set; } = string.Empty;
    public string SourceVersion { get; set; } = string.Empty;
    public string SourceChecksum { get; set; } = string.Empty;
    
    // Claims and roles from external system
    public Dictionary<string, string> Claims { get; set; } = new();
    public List<string> Roles { get; set; } = new();
    
    // Profile snapshot for audit
    public string? ProfileSnapshot { get; set; }
    
    // Watermark for incremental processing
    public UserSeedWatermark Watermark { get; set; } = new();
    
    // Validation metadata
    public bool IsValid { get; set; } = true;
    public List<string> ValidationErrors { get; set; } = new();
    public List<string> ValidationWarnings { get; set; } = new();
}

/// <summary>
/// Watermark for incremental processing of user seed records
/// </summary>
public class UserSeedWatermark
{
    public string? LastProcessedId { get; set; }
    public DateTime? LastProcessedUpdatedAt { get; set; }
    public string? LastProcessedChecksum { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Result of user seed record validation
/// </summary>
public class UserSeedValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}