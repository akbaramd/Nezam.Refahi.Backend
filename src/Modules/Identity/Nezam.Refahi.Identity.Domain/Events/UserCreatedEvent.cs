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