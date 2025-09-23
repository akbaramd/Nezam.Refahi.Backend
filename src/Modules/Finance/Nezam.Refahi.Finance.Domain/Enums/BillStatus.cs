using System.Text.Json.Serialization;

namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Status of a bill
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
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
    Refunded = 7,

    /// <summary>
    /// Bill is voided before any payment; differs from Cancel (prevents financial impact)
    /// </summary>
    Voided = 8,

    /// <summary>
    /// Bad debt written off (with financial/treasury documentation)
    /// </summary>
    WrittenOff = 9,

    /// <summary>
    /// Credit note issued for correction/post-payment discount; equivalent to Refund at document level
    /// </summary>
    Credited = 10,

    /// <summary>
    /// Under financial dispute; suspended until arbitration
    /// </summary>
    Disputed = 11
}