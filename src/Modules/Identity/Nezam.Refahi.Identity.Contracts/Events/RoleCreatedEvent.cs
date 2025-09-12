using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Contracts.Events;

/// <summary>
/// Domain event raised when a role is created
/// </summary>
public class RoleCreatedEvent : DomainEvent
{
    public Guid RoleId { get; }
    public string RoleName { get; }
    public bool IsSystemRole { get; }
    public DateTime OccurredAt { get; }

    public RoleCreatedEvent(Guid roleId, string roleName, bool isSystemRole)
    {
        RoleId = roleId;
        RoleName = roleName;
        IsSystemRole = isSystemRole;
        OccurredAt = DateTime.UtcNow;
    }
}