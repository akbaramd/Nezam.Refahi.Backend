using MCA.SharedKernel.Domain.Contracts;
using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Notifications.Domain.Events;

/// <summary>
/// Domain event raised when a notification is created
/// </summary>
public class NotificationCreatedEvent : DomainEvent
{
    public Guid NotificationId { get; }
    public Guid ExternalUserId { get; }
    public string Title { get; }
    public DateTime CreatedAt { get; }
    
    public NotificationCreatedEvent(
        Guid notificationId, 
        Guid externalUserId, 
        string title)
    {
        NotificationId = notificationId;
        ExternalUserId = externalUserId;
        Title = title;
        CreatedAt = DateTime.UtcNow;
    }
}
