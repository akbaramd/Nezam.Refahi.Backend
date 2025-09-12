using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when a user preference is added
/// </summary>
public class UserPreferenceAddedEvent : DomainEvent
{
  public Guid UserId { get; }
  public string PreferenceKey { get; }
  public string PreferenceValue { get; }
  public string Category { get; }
  public DateTime OccurredAt { get; }

  public UserPreferenceAddedEvent(Guid userId, string preferenceKey, string preferenceValue, string category)
  {
    UserId = userId;
    PreferenceKey = preferenceKey;
    PreferenceValue = preferenceValue;
    Category = category;
    OccurredAt = DateTime.UtcNow;
  }
}