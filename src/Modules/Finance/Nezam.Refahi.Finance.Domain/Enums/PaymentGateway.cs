namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Payment gateway providers
/// </summary>
public enum PaymentGateway
{
    /// <summary>
    /// Zarinpal payment gateway
    /// </summary>
    Zarinpal = 1,

    /// <summary>
    /// Mellat Bank payment gateway
    /// </summary>
    Mellat = 2,

    /// <summary>
    /// Parsian Bank payment gateway
    /// </summary>
    Parsian = 3,

    /// <summary>
    /// Saman Bank payment gateway
    /// </summary>
    Saman = 4,

    /// <summary>
    /// Pasargad Bank payment gateway
    /// </summary>
    Pasargad = 5,

    /// <summary>
    /// Iran Kish payment gateway
    /// </summary>
    IranKish = 6,
    Wallet = 7
}