using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// Domain event raised when wallet status changes
/// </summary>
        public sealed class WalletStatusChangedEvent(
    Guid WalletId,
    Guid ExternalUserId,
    WalletStatus PreviousStatus,
    WalletStatus NewStatus,
    string? Reason,
    DateTime ChangedAt) : DomainEvent
{
    public Guid WalletId { get; } = WalletId;
    public Guid ExternalUserId { get; } = ExternalUserId;
    public WalletStatus PreviousStatus { get; } = PreviousStatus;
    public WalletStatus NewStatus { get; } = NewStatus;
    public string? Reason { get; } = Reason;
    public DateTime ChangedAt { get; } = ChangedAt;
}
