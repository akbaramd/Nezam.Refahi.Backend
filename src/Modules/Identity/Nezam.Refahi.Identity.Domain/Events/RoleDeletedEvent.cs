using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a role is deleted
/// </summary>
public class RoleDeletedEvent : DomainEvent
{
  public Guid RoleId { get; }
  public string RoleName { get; }
  public DateTime OccurredAt { get; }

  public RoleDeletedEvent(Guid roleId, string roleName)
  {
    RoleId = roleId;
    RoleName = roleName;
    OccurredAt = DateTime.UtcNow;
  }
}