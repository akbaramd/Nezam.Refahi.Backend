using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when a role is assigned to a user
/// </summary>
public class UserRoleAssignedEvent : DomainEvent
{
  public Guid UserId { get; }
  public Guid RoleId { get; }
  public string? AssignedBy { get; }
  public DateTime? ExpiresAt { get; }
  public DateTime OccurredAt { get; }

  public UserRoleAssignedEvent(Guid userId, Guid roleId, string? assignedBy = null, DateTime? expiresAt = null)
  {
    UserId = userId;
    RoleId = roleId;
    AssignedBy = assignedBy;
    ExpiresAt = expiresAt;
    OccurredAt = DateTime.UtcNow;
  }
}