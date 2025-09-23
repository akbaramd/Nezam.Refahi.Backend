using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a wallet transaction is completed
/// </summary>
public sealed class WalletTransactionCompletedEvent(
    Guid TransactionId,
    Guid WalletId,
    string NationalNumber,
    WalletTransactionType TransactionType,
    Money Amount,
    Money NewBalance,
    string? ReferenceId,
    string? Description,
    DateTime CompletedAt) : DomainEvent
{
    public Guid TransactionId { get; } = TransactionId;
    public Guid WalletId { get; } = WalletId;
    public string NationalNumber { get; } = NationalNumber;
    public WalletTransactionType TransactionType { get; } = TransactionType;
    public Money Amount { get; } = Amount;
    public Money NewBalance { get; } = NewBalance;
    public string? ReferenceId { get; } = ReferenceId;
    public string? Description { get; } = Description;
    public DateTime CompletedAt { get; } = CompletedAt;
}
