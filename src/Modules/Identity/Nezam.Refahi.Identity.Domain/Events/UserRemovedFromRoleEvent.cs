using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user is removed from a role
/// </summary>
public class UserRemovedFromRoleEvent : DomainEvent
{
  public Guid UserId { get; }
  public Guid RoleId { get; }
  public string RoleName { get; }
  public string? RemovedBy { get; }
  public DateTime OccurredAt { get; }

  public UserRemovedFromRoleEvent(Guid userId, Guid roleId, string roleName, string? removedBy = null)
  {
    UserId = userId;
    RoleId = roleId;
    RoleName = roleName;
    RemovedBy = removedBy;
    OccurredAt = DateTime.UtcNow;
  }
}