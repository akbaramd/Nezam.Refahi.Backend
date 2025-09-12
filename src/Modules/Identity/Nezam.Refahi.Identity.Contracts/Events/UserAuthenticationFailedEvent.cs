using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when a user fails authentication
/// </summary>
public class UserAuthenticationFailedEvent : DomainEvent
{
  public Guid UserId { get; }
  public int FailedAttempts { get; }
  public bool IsLockedAfterFailure { get; }
  public DateTime OccurredAt { get; }

  public UserAuthenticationFailedEvent(Guid userId, int failedAttempts, bool isLockedAfterFailure = false)
  {
    UserId = userId;
    FailedAttempts = failedAttempts;
    IsLockedAfterFailure = isLockedAfterFailure;
    OccurredAt = DateTime.UtcNow;
  }
}