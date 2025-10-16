using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user is unlocked
/// </summary>
public class UserUnlockedEvent : DomainEvent
{
  public Guid UserId { get; }
  public string Reason { get; }
  public DateTime OccurredAt { get; }

  public UserUnlockedEvent(Guid userId, string reason)
  {
    UserId = userId;
    Reason = reason;
    OccurredAt = DateTime.UtcNow;
  }
}