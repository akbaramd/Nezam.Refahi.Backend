using Microsoft.Extensions.Logging;
using Nezam.Refahi.Application.Common.Interfaces;

namespace Nezam.Refahi.Infrastructure.Services;

/// <summary>
/// Service for sending notifications to users
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendSmsAsync(string phoneNumber, string message)
    {
        // In a real implementation, this would integrate with an SMS gateway provider
        // For now, we'll just log the message
        _logger.LogInformation("SMS sent to {PhoneNumber}: {Message}", phoneNumber, message);
        
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        // In a real implementation, this would integrate with an email service provider
        // For now, we'll just log the message
        _logger.LogInformation("Email sent to {Email} with subject {Subject}: {Message}", email, subject, message);
        
        return Task.CompletedTask;
    }
}
