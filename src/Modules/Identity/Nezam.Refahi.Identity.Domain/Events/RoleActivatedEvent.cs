using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a role is activated
/// </summary>
public class RoleActivatedEvent : DomainEvent
{
  public Guid RoleId { get; }
  public string RoleName { get; }
  public DateTime OccurredAt { get; }

  public RoleActivatedEvent(Guid roleId, string roleName)
  {
    RoleId = roleId;
    RoleName = roleName;
    OccurredAt = DateTime.UtcNow;
  }
}