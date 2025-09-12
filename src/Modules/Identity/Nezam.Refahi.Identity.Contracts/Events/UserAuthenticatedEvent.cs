using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when a user successfully authenticates
/// </summary>
public class UserAuthenticatedEvent : DomainEvent
{
  public Guid UserId { get; }
  public string? IpAddress { get; }
  public string? UserAgent { get; }
  public string? DeviceFingerprint { get; }
  public DateTime OccurredAt { get; }

  public UserAuthenticatedEvent(Guid userId, string? ipAddress = null, string? userAgent = null, string? deviceFingerprint = null)
  {
    UserId = userId;
    IpAddress = ipAddress;
    UserAgent = userAgent;
    DeviceFingerprint = deviceFingerprint;
    OccurredAt = DateTime.UtcNow;
  }
}