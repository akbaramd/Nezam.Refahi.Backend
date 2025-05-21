namespace Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects
{
    /// <summary>
    /// Represents the method used for a payment transaction.
    /// </summary>
    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        BankTransfer,
        DigitalWallet,
        Cash,
        Check,
        CryptoCurrency,
        GiftCard,
        MobilePayment,
        Other
    }
}
