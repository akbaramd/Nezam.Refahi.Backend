using System.Text.Json.Serialization;

namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Status of a wallet deposit request
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WalletDepositStatus
{
    /// <summary>
    /// Deposit request is pending payment
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Deposit is processing payment
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Deposit is completed successfully
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Deposit failed
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Deposit is cancelled
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// Deposit is expired
    /// </summary>
    Expired = 6
}
