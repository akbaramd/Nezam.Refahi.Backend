using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a claim is removed from a role
/// </summary>
public class RoleClaimRemovedEvent : DomainEvent
{
  public Guid RoleId { get; }
  public string RoleName { get; }
  public string ClaimType { get; }
  public string ClaimValue { get; }
  public DateTime OccurredAt { get; }

  public RoleClaimRemovedEvent(Guid roleId, string roleName, string claimType, string claimValue)
  {
    RoleId = roleId;
    RoleName = roleName;
    ClaimType = claimType;
    ClaimValue = claimValue;
    OccurredAt = DateTime.UtcNow;
  }
}