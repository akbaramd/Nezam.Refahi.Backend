using System.Text.Json.Serialization;

namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Type of wallet transaction
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WalletTransactionType
{
    /// <summary>
    /// Money deposited into wallet
    /// </summary>
    Deposit = 1,

    /// <summary>
    /// Money withdrawn from wallet
    /// </summary>
    Withdrawal = 2,

    /// <summary>
    /// Transfer to another wallet
    /// </summary>
    TransferOut = 3,

    /// <summary>
    /// Transfer received from another wallet
    /// </summary>
    TransferIn = 4,

    /// <summary>
    /// Payment made from wallet to bill
    /// </summary>
    Payment = 5,

    /// <summary>
    /// Refund received into wallet
    /// </summary>
    Refund = 6,

    /// <summary>
    /// Administrative adjustment (credit/debit)
    /// </summary>
    Adjustment = 7,

    /// <summary>
    /// Interest or bonus credited
    /// </summary>
    Interest = 8,

    /// <summary>
    /// Fee charged
    /// </summary>
    Fee = 9
}
