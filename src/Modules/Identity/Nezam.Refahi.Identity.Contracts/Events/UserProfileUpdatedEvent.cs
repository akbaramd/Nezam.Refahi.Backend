using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when a user's profile is updated
/// </summary>
public class UserProfileUpdatedEvent : DomainEvent
{
  public Guid UserId { get; }
  public string FirstName { get; }
  public string LastName { get; }
  public string? NationalId { get; }
  public DateTime OccurredAt { get; }

  public UserProfileUpdatedEvent(Guid userId, string firstName, string lastName, string? nationalId)
  {
    UserId = userId;
    FirstName = firstName;
    LastName = lastName;
    NationalId = nationalId;
    OccurredAt = DateTime.UtcNow;
  }
}