using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when wallet balance changes
/// </summary>
public sealed class WalletBalanceChangedEvent(
    Guid WalletId,
    string NationalNumber,
    WalletTransactionType TransactionType,
    Money PreviousBalance,
    Money NewBalance,
    Money TransactionAmount,
    string? ReferenceId,
    DateTime ChangedAt) : DomainEvent
{
    public Guid WalletId { get; } = WalletId;
    public string NationalNumber { get; } = NationalNumber;
    public WalletTransactionType TransactionType { get; } = TransactionType;
    public Money PreviousBalance { get; } = PreviousBalance;
    public Money NewBalance { get; } = NewBalance;
    public Money TransactionAmount { get; } = TransactionAmount;
    public string? ReferenceId { get; } = ReferenceId;
    public DateTime ChangedAt { get; } = ChangedAt;
}
