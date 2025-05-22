using System.Threading.Tasks;

namespace Nezam.Refahi.Application.Common.Interfaces;

/// <summary>
/// Service for sending notifications to users
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends an SMS notification to a phone number
    /// </summary>
    /// <param name="phoneNumber">The recipient's phone number</param>
    /// <param name="message">The message content</param>
    Task SendSmsAsync(string phoneNumber, string message);
    
    /// <summary>
    /// Sends an email notification
    /// </summary>
    /// <param name="email">The recipient's email address</param>
    /// <param name="subject">The email subject</param>
    /// <param name="message">The email content</param>
    Task SendEmailAsync(string email, string subject, string message);
}
