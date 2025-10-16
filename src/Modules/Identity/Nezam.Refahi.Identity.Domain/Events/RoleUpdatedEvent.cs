using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a role is updated
/// </summary>
public class RoleUpdatedEvent : DomainEvent
{
  public Guid RoleId { get; }
  public string OldName { get; }
  public string NewName { get; }
  public DateTime OccurredAt { get; }

  public RoleUpdatedEvent(Guid roleId, string oldName, string newName)
  {
    RoleId = roleId;
    OldName = oldName;
    NewName = newName;
    OccurredAt = DateTime.UtcNow;
  }
}