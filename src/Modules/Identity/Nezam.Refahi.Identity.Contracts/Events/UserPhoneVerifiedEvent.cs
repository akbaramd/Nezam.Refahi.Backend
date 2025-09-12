using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when a user's phone is verified
/// </summary>
public class UserPhoneVerifiedEvent : DomainEvent
{
  public Guid UserId { get; }
  public string PhoneNumber { get; }
  public DateTime OccurredAt { get; }

  public UserPhoneVerifiedEvent(Guid userId, string phoneNumber)
  {
    UserId = userId;
    PhoneNumber = phoneNumber;
    OccurredAt = DateTime.UtcNow;
  }
}