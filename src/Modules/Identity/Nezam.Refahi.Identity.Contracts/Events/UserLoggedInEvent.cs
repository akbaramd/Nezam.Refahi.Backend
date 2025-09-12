using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when a user logs in
/// </summary>
public class UserLoggedInEvent : DomainEvent
{
  public Guid UserId { get; }
  public DateTime OccurredAt { get; }

  public UserLoggedInEvent(Guid userId)
  {
    UserId = userId;
    OccurredAt = DateTime.UtcNow;
  }
}