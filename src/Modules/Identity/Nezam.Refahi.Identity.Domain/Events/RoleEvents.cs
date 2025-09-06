using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Events;

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

/// <summary>
/// Domain event raised when a role is deactivated
/// </summary>
public class RoleDeactivatedEvent : DomainEvent
{
    public Guid RoleId { get; }
    public string RoleName { get; }
    public DateTime OccurredAt { get; }

    public RoleDeactivatedEvent(Guid roleId, string roleName)
    {
        RoleId = roleId;
        RoleName = roleName;
        OccurredAt = DateTime.UtcNow;
    }
}

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
