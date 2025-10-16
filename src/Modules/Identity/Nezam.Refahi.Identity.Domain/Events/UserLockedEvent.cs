using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user is locked
/// </summary>
public class UserLockedEvent : DomainEvent
{
  public Guid UserId { get; }
  public string Reason { get; }
  public DateTime? UnlockAt { get; }
  public DateTime OccurredAt { get; }

  public UserLockedEvent(Guid userId, string reason, DateTime? unlockAt = null)
  {
    UserId = userId;
    Reason = reason;
    UnlockAt = unlockAt;
    OccurredAt = DateTime.UtcNow;
  }
}