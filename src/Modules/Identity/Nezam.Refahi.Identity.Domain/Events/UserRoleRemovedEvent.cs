using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a role is removed from a user
/// </summary>
public class UserRoleRemovedEvent : DomainEvent
{
  public Guid UserId { get; }
  public Guid RoleId { get; }
  public DateTime OccurredAt { get; }

  public UserRoleRemovedEvent(Guid userId, Guid roleId)
  {
    UserId = userId;
    RoleId = roleId;
    OccurredAt = DateTime.UtcNow;
  }
}