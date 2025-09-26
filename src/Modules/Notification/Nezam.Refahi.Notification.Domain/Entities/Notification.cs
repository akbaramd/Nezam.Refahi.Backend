using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Notifications.Domain.Entities;

/// <summary>
/// Represents a notification in the system
/// Simple entity with external user ID reference
/// </summary>
public class Notification : Entity<Guid>
{
    public Guid ExternalUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string Context { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    
    // JSON data for additional information
    public string? Data { get; private set; }
    
    // Private constructor for EF Core
    private Notification() { }
    
    public Notification(
        Guid externalUserId, 
        string title, 
        string message, 
        string context,
        string action,
        string? data = null,
        DateTime? expiresAt = null)
    {
        if (externalUserId == Guid.Empty)
            throw new ArgumentException("External user ID cannot be empty", nameof(externalUserId));
            
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
            
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));
            
        if (string.IsNullOrWhiteSpace(context))
            throw new ArgumentException("Context cannot be empty", nameof(context));
            
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));
            
        if (title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters", nameof(title));
            
        if (message.Length > 1000)
            throw new ArgumentException("Message cannot exceed 1000 characters", nameof(message));
            
        if (context.Length > 100)
            throw new ArgumentException("Context cannot exceed 100 characters", nameof(context));
            
        if (action.Length > 100)
            throw new ArgumentException("Action cannot exceed 100 characters", nameof(action));
    
        Id = Guid.NewGuid();
        ExternalUserId = externalUserId;
        Title = title;
        Message = message;
        Context = context;
        Action = action;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        Data = data;
    }
    
    /// <summary>
    /// Marks the notification as read
    /// </summary>
    public void MarkAsRead()
    {
        if (IsRead)
            return;
            
        IsRead = true;
    }
    
    /// <summary>
    /// Checks if the notification is expired
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    
    /// <summary>
    /// Checks if the notification has an action
    /// </summary>
    public bool HasAction => !string.IsNullOrWhiteSpace(Action);
    
    /// <summary>
    /// Updates the notification data
    /// </summary>
    public void UpdateData(string? data)
    {
        Data = data;
    }
    
    /// <summary>
    /// Updates the action
    /// </summary>
    public void UpdateAction(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));
            
        if (action.Length > 100)
            throw new ArgumentException("Action cannot exceed 100 characters", nameof(action));
            
        Action = action;
    }
    
    /// <summary>
    /// Updates the context
    /// </summary>
    public void UpdateContext(string context)
    {
        if (string.IsNullOrWhiteSpace(context))
            throw new ArgumentException("Context cannot be empty", nameof(context));
            
        if (context.Length > 100)
            throw new ArgumentException("Context cannot exceed 100 characters", nameof(context));
            
        Context = context;
    }
}