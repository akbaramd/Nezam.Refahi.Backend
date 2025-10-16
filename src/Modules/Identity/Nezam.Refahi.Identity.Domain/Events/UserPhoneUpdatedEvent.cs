using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user's phone number is updated
/// </summary>
public class UserPhoneUpdatedEvent : DomainEvent
{
  public Guid UserId { get; }
  public string OldPhoneNumber { get; }
  public string NewPhoneNumber { get; }
  public DateTime OccurredAt { get; }

  public UserPhoneUpdatedEvent(Guid userId, string oldPhoneNumber, string newPhoneNumber)
  {
    UserId = userId;
    OldPhoneNumber = oldPhoneNumber;
    NewPhoneNumber = newPhoneNumber;
    OccurredAt = DateTime.UtcNow;
  }
}