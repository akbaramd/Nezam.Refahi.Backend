using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user is assigned to a role
/// </summary>
public class UserAssignedToRoleEvent : DomainEvent
{
  public Guid UserId { get; }
  public Guid RoleId { get; }
  public string RoleName { get; }
  public string? AssignedBy { get; }
  public DateTime? ExpiresAt { get; }
  public DateTime OccurredAt { get; }

  public UserAssignedToRoleEvent(Guid userId, Guid roleId, string roleName, string? assignedBy = null, DateTime? expiresAt = null)
  {
    UserId = userId;
    RoleId = roleId;
    RoleName = roleName;
    AssignedBy = assignedBy;
    ExpiresAt = expiresAt;
    OccurredAt = DateTime.UtcNow;
  }
}