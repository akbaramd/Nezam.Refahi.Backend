using System.Text.Json.Serialization;

namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Status of a wallet deposit request
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WalletDepositStatus
{
    /// <summary>
    /// DEPRECATED: ambiguous. Prefer Requested/AwaitingBill/AwaitingPayment.
    /// </summary>
    Requested = 1,

    /// <summary>
    /// DEPRECATED: ambiguous. Prefer AwaitingPayment/AwaitingCredit.
    /// </summary>
    AwaitingBill = 2,
    AwaitingPayment = 3,
    /// <summary>
    /// Deposit is completed successfully
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Deposit failed
    /// </summary>
    Failed = 5,

    /// <summary>
    /// Deposit is cancelled
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Deposit is expired
    /// </summary>
    Expired = 7,


}
