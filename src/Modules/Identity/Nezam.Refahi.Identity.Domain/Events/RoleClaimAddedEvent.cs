using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a claim is added to a role
/// </summary>
public class RoleClaimAddedEvent : DomainEvent
{
  public Guid RoleId { get; }
  public string RoleName { get; }
  public string ClaimType { get; }
  public string ClaimValue { get; }
  public DateTime OccurredAt { get; }

  public RoleClaimAddedEvent(Guid roleId, string roleName, string claimType, string claimValue)
  {
    RoleId = roleId;
    RoleName = roleName;
    ClaimType = claimType;
    ClaimValue = claimValue;
    OccurredAt = DateTime.UtcNow;
  }
}