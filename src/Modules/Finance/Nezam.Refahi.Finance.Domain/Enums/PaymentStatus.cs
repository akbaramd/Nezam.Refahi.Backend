namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Status of a payment transaction
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment is pending processing
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Payment is being processed by gateway
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Payment completed successfully
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Payment failed
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Payment was cancelled by user
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// Payment expired due to timeout
    /// </summary>
    Expired = 6,

    /// <summary>
    /// Payment was refunded
    /// </summary>
    Refunded = 7
}