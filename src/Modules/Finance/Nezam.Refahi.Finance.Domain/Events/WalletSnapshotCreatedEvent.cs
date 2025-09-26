using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when a wallet snapshot is created
/// </summary>
public class WalletSnapshotCreatedEvent : DomainEvent
{
    public Guid SnapshotId { get; }
    public Guid WalletId { get; }
    public Guid ExternalUserId { get; }
    public Money Balance { get; }
    public DateTime SnapshotDate { get; }
    public int TransactionCount { get; }

    public WalletSnapshotCreatedEvent(
        Guid snapshotId,
        Guid walletId,
        Guid externalUserId,
        Money balance,
        DateTime snapshotDate,
        int transactionCount)
    {
        SnapshotId = snapshotId;
        WalletId = walletId;
        ExternalUserId = externalUserId;
        Balance = balance;
        SnapshotDate = snapshotDate;
        TransactionCount = transactionCount;
    }
}
