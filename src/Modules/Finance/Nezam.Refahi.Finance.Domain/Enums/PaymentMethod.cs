namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Payment method types
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Online payment through gateway
    /// </summary>
    Online = 1,

    /// <summary>
    /// Bank transfer
    /// </summary>
    BankTransfer = 2,

    /// <summary>
    /// Cash payment
    /// </summary>
    Cash = 3,

    /// <summary>
    /// Card to card transfer
    /// </summary>
    CardToCard = 4,

    /// <summary>
    /// Wallet payment
    /// </summary>
    Wallet = 5
}