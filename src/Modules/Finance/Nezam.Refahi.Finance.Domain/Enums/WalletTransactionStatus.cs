using System.Text.Json.Serialization;

namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Status of a wallet transaction
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WalletTransactionStatus
{
    /// <summary>
    /// Transaction is pending processing
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Transaction is processing
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Transaction is completed successfully
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Transaction failed
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Transaction is cancelled
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// Transaction is refunded
    /// </summary>
    Refunded = 6
}
