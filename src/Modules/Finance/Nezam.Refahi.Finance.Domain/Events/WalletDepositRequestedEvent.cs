using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a wallet deposit is requested
/// </summary>
public class WalletDepositRequestedEvent : DomainEvent
{
    public Guid DepositId { get; }
    public Guid WalletId { get; }
    public Guid ExternalUserId { get; }
    public Money Amount { get; }
    public DateTime RequestedAt { get; }
    public string? Description { get; }

    public WalletDepositRequestedEvent(
        Guid depositId,
        Guid walletId,
        Guid externalUserId,
        Money amount,
        DateTime requestedAt,
        string? description = null)
    {
        DepositId = depositId;
        WalletId = walletId;
        ExternalUserId = externalUserId;
        Amount = amount;
        RequestedAt = requestedAt;
        Description = description;
    }
}
