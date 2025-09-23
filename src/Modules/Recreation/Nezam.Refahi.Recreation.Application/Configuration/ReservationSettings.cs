namespace Nezam.Refahi.Recreation.Application.Configuration;

/// <summary>
/// Configuration settings for tour reservations
/// </summary>
public class ReservationSettings
{
    public const string SectionName = "ReservationSettings";

    /// <summary>
    /// Duration in minutes for holding a reservation before it expires
    /// Default: 15 minutes
    /// </summary>
    public int ReservationHoldMinutes { get; set; } = 15;

    /// <summary>
    /// Minimum hours before tour start time to allow reservations
    /// Default: 24 hours
    /// </summary>
    public int MinimumHoursBeforeTour { get; set; } = 24;

    /// <summary>
    /// Default tenant ID for multi-tenant scenarios
    /// </summary>
    public string DefaultTenantId { get; set; } = "default";

    /// <summary>
    /// TTL for idempotency records in minutes
    /// Default: 30 minutes
    /// </summary>
    public int IdempotencyTtlMinutes { get; set; } = 30;

    /// <summary>
    /// Maximum age considered reasonable for participants
    /// Default: 120 years
    /// </summary>
    public int MaxReasonableAge { get; set; } = 120;

    /// <summary>
    /// Grace period in minutes for expired reservation cleanup
    /// Default: 5 minutes
    /// </summary>
    public int GracePeriodMinutes { get; set; } = 5;

    /// <summary>
    /// Grace period in minutes for payment callbacks after reservation expiry
    /// Default: 10 minutes (total: 15 min payment + 5 min grace + 10 min callback = 30 min)
    /// </summary>
    public int PaymentCallbackGracePeriodMinutes { get; set; } = 10;

    /// <summary>
    /// Cleanup interval in minutes for background service
    /// Default: 10 minutes
    /// </summary>
    public int CleanupIntervalMinutes { get; set; } = 10;

    /// <summary>
    /// Error retry interval in minutes for background service
    /// Default: 2 minutes
    /// </summary>
    public int ErrorRetryIntervalMinutes { get; set; } = 2;
}
