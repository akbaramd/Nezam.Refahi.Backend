using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Services.Contracts;

/// <summary>
/// Service for managing OTP challenge cleanup operations
/// Handles bulk data cleanup scenarios and prevents system overload
/// </summary>
public interface IOtpCleanupService
{
    /// <summary>
    /// Performs a full cleanup of consumed and expired challenges
    /// Designed for background processing and big data scenarios
    /// </summary>
    /// <param name="batchSize">Number of records to process per batch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total number of cleaned up challenges</returns>
    Task<int> PerformFullCleanupAsync(int batchSize = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up challenges older than specified days
    /// Use for archival and long-term data management
    /// </summary>
    /// <param name="daysOld">Days old threshold</param>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of deleted challenges</returns>
    Task<int> CleanupOldChallengesAsync(int daysOld = 30, int batchSize = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Emergency cleanup for specific phone number
    /// Use when a user reports issues or data needs to be reset
    /// </summary>
    /// <param name="phoneNumber">Phone number to clean</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of deleted challenges</returns>
    Task<int> EmergencyCleanupForPhoneAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Optimized cleanup during off-peak hours
    /// Combines multiple cleanup strategies for maximum efficiency
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cleanup statistics</returns>
    Task<CleanupStats> PerformOptimizedMaintenanceAsync(CancellationToken cancellationToken = default);
}