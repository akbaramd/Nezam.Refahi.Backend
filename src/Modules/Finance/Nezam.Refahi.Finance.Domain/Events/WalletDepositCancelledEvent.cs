using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a wallet deposit is cancelled
/// </summary>
public class WalletDepositCancelledEvent : DomainEvent
{
    public Guid DepositId { get; }
    public Guid WalletId { get; }
    public string UserNationalNumber { get; }
    public Money Amount { get; }
    public DateTime CancelledAt { get; }
    public string? Reason { get; }

    public WalletDepositCancelledEvent(
        Guid depositId,
        Guid walletId,
        string userNationalNumber,
        Money amount,
        DateTime cancelledAt,
        string? reason = null)
    {
        DepositId = depositId;
        WalletId = walletId;
        UserNationalNumber = userNationalNumber;
        Amount = amount;
        CancelledAt = cancelledAt;
        Reason = reason;
    }
}
