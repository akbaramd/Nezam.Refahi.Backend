using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Notifications.Application.Services;

/// <summary>
/// Unit of Work interface for Notification module
/// Manages database transactions and coordinates domain event publishing
/// </summary>
public interface INotificationUnitOfWork : IUnitOfWork
{
}
