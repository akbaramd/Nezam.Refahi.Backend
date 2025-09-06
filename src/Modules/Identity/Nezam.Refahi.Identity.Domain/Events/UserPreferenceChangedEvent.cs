using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Identity.Domain.Services;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user preference is changed
/// </summary>
public class UserPreferenceChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string PreferenceKey { get; }
    public string OldValue { get; }
    public string NewValue { get; }
    public PreferenceCategory Category { get; }
    public DateTime OccurredAt { get; }

    public UserPreferenceChangedEvent(
        Guid userId, 
        string preferenceKey, 
        string oldValue, 
        string newValue, 
        PreferenceCategory category)
    {
        UserId = userId;
        PreferenceKey = preferenceKey;
        OldValue = oldValue;
        NewValue = newValue;
        Category = category;
        OccurredAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Domain event raised when a user preference is created
/// </summary>
public class UserPreferenceCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string PreferenceKey { get; }
    public string Value { get; }
    public PreferenceCategory Category { get; }
    public DateTime OccurredAt { get; }

    public UserPreferenceCreatedEvent(
        Guid userId, 
        string preferenceKey, 
        string value, 
        PreferenceCategory category)
    {
        UserId = userId;
        PreferenceKey = preferenceKey;
        Value = value;
        Category = category;
        OccurredAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Domain event raised when a user preference is deleted
/// </summary>
public class UserPreferenceDeletedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string PreferenceKey { get; }
    public string Value { get; }
    public PreferenceCategory Category { get; }
    public DateTime OccurredAt { get; }

    public UserPreferenceDeletedEvent(
        Guid userId, 
        string preferenceKey, 
        string value, 
        PreferenceCategory category)
    {
        UserId = userId;
        PreferenceKey = preferenceKey;
        Value = value;
        Category = category;
        OccurredAt = DateTime.UtcNow;
    }
}
