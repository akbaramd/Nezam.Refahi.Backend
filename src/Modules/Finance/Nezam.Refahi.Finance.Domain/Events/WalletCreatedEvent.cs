using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a new wallet is created
/// </summary>
public sealed class WalletCreatedEvent(
    Guid WalletId,
    Guid ExternalUserId,
    Money InitialBalance,
    DateTime CreatedAt) : DomainEvent
{
    public Guid WalletId { get; } = WalletId;
    public Guid ExternalUserId { get; } = ExternalUserId;
    public Money InitialBalance { get; } = InitialBalance;
    public DateTime CreatedAt { get; } = CreatedAt;
}
