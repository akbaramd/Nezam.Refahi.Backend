using MCA.SharedKernel.Domain.Contracts;
using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Notifications.Domain.Events;

/// <summary>
/// Domain event raised when a notification is marked as read
/// </summary>
public class NotificationReadEvent : DomainEvent
{
    public Guid NotificationId { get; }
    public Guid ExternalUserId { get; }
    public DateTime ReadAt { get; }
    
    public NotificationReadEvent(Guid notificationId, Guid externalUserId)
    {
        NotificationId = notificationId;
        ExternalUserId = externalUserId;
        ReadAt = DateTime.UtcNow;
    }
}
