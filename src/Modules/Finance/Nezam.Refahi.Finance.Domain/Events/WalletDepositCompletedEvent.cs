using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a wallet deposit is completed
/// </summary>
public class WalletDepositCompletedEvent : DomainEvent
{
    public Guid DepositId { get; }
    public Guid WalletId { get; }
    public string UserNationalNumber { get; }
    public Money Amount { get; }
    public DateTime CompletedAt { get; }
    public string? Description { get; }

    public WalletDepositCompletedEvent(
        Guid depositId,
        Guid walletId,
        string userNationalNumber,
        Money amount,
        DateTime completedAt,
        string? description = null)
    {
        DepositId = depositId;
        WalletId = walletId;
        UserNationalNumber = userNationalNumber;
        Amount = amount;
        CompletedAt = completedAt;
        Description = description;
    }
}
