using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.Contracts;
using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user is created
/// </summary>
public class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string PhoneNumber { get; }
    public string? NationalId { get; }
    public DateTime OccurredAt { get; }

    public UserCreatedEvent(Guid userId, string phoneNumber, string? nationalId = null)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
        NationalId = nationalId;
        OccurredAt = DateTime.UtcNow;
    }
}

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
