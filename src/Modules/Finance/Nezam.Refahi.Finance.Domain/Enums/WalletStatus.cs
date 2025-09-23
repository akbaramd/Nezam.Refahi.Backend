using System.Text.Json.Serialization;

namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Status of a wallet
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WalletStatus
{
    /// <summary>
    /// Wallet is active and can perform transactions
    /// </summary>
    Active = 1,

    /// <summary>
    /// Wallet is suspended temporarily (cannot perform transactions)
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Wallet is frozen (administrative action, requires manual intervention)
    /// </summary>
    Frozen = 3,

    /// <summary>
    /// Wallet is closed permanently
    /// </summary>
    Closed = 4
}
