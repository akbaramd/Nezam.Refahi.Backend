using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when a role is deactivated
/// </summary>
public class RoleDeactivatedEvent : DomainEvent
{
  public Guid RoleId { get; }
  public string RoleName { get; }
  public DateTime OccurredAt { get; }

  public RoleDeactivatedEvent(Guid roleId, string roleName)
  {
    RoleId = roleId;
    RoleName = roleName;
    OccurredAt = DateTime.UtcNow;
  }
}