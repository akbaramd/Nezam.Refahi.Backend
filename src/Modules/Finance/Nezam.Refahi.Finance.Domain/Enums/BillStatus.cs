namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Status of a bill
/// </summary>
public enum BillStatus
{
    /// <summary>
    /// Bill is drafted but not issued yet
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Bill is issued and pending payment
    /// </summary>
    Issued = 2,

    /// <summary>
    /// Bill is partially paid
    /// </summary>
    PartiallyPaid = 3,

    /// <summary>
    /// Bill is fully paid
    /// </summary>
    FullyPaid = 4,

    /// <summary>
    /// Bill is overdue
    /// </summary>
    Overdue = 5,

    /// <summary>
    /// Bill is cancelled
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Bill is refunded
    /// </summary>
    Refunded = 7
}