namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Status of a refund transaction
/// </summary>
public enum RefundStatus
{
    /// <summary>
    /// Refund request is pending review
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Refund request approved and being processed
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Refund completed successfully
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Refund request rejected
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Refund failed during processing
    /// </summary>
    Failed = 5
}