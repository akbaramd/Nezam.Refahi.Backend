using MCA.SharedKernel.Domain.AggregateRoots;
using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Identity.Contracts.Events;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Entities;

/// <summary>
/// Represents a role in the system with associated claims
/// </summary>
public class Role : FullAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystemRole { get; private set; }
    public int DisplayOrder { get; private set; }
    
    // Navigation properties
    private readonly List<RoleClaim> _claims = new();
    public IReadOnlyCollection<RoleClaim> Claims => _claims.AsReadOnly();
    
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    // Private constructor for EF Core
    public Role() : base() { }

    public Role(string name, string? description = null, bool isSystemRole = false, int displayOrder = 0) : base(Guid.NewGuid())
    {
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Role name cannot exceed 100 characters", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        IsActive = true;
        IsSystemRole = isSystemRole;
        DisplayOrder = displayOrder;

        // Raise domain event
        AddDomainEvent(new RoleCreatedEvent(Id, Name, IsSystemRole));
    }

    /// <summary>
    /// Updates role details
    /// </summary>
    public void UpdateDetails(string name, string? description = null, int displayOrder = 0)
    {
        if (IsSystemRole)
            throw new InvalidOperationException("Cannot modify system role");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Role name cannot exceed 100 characters", nameof(name));

        var oldName = Name;
        Name = name.Trim();
        Description = description?.Trim();
        DisplayOrder = displayOrder;

        // Raise domain event
        AddDomainEvent(new RoleUpdatedEvent(Id, oldName, Name));
    }

    /// <summary>
    /// Activates the role
    /// </summary>
    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            AddDomainEvent(new RoleActivatedEvent(Id, Name));
        }
    }

    /// <summary>
    /// Deactivates the role
    /// </summary>
    public void Deactivate()
    {
        if (IsSystemRole)
            throw new InvalidOperationException("Cannot deactivate system role");

        if (IsActive)
        {
            IsActive = false;
            AddDomainEvent(new RoleDeactivatedEvent(Id, Name));
        }
    }

    /// <summary>
    /// Adds a claim to the role
    /// </summary>
    public void AddClaim(Claim claim)
    {
        if (claim == null)
            throw new ArgumentNullException(nameof(claim));

        if (_claims.Any(c => c.Claim.Type == claim.Type && c.Claim.Value == claim.Value))
            throw new InvalidOperationException($"Claim '{claim}' already exists in role '{Name}'");

        var roleClaim = new RoleClaim(Id, claim);
        _claims.Add(roleClaim);

        AddDomainEvent(new RoleClaimAddedEvent(Id, Name, claim.Type, claim.Value));
    }

    /// <summary>
    /// Removes a claim from the role
    /// </summary>
    public void RemoveClaim(Claim claim)
    {
        if (claim == null)
            throw new ArgumentNullException(nameof(claim));

        var roleClaim = _claims.FirstOrDefault(c => c.Claim.Type == claim.Type && c.Claim.Value == claim.Value);
        if (roleClaim != null)
        {
            _claims.Remove(roleClaim);
            AddDomainEvent(new RoleClaimRemovedEvent(Id, Name, claim.Type, claim.Value));
        }
    }

    /// <summary>
    /// Removes a claim by type and value
    /// </summary>
    public void RemoveClaim(string claimType, string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            throw new ArgumentException("Claim type cannot be empty", nameof(claimType));

        if (string.IsNullOrWhiteSpace(claimValue))
            throw new ArgumentException("Claim value cannot be empty", nameof(claimValue));

        var roleClaim = _claims.FirstOrDefault(c => c.Claim.Type == claimType && c.Claim.Value == claimValue);
        if (roleClaim != null)
        {
            _claims.Remove(roleClaim);
            AddDomainEvent(new RoleClaimRemovedEvent(Id, Name, claimType, claimValue));
        }
    }

    /// <summary>
    /// Checks if the role has a specific claim
    /// </summary>
    public bool HasClaim(Claim claim)
    {
        if (claim == null)
            return false;

        return _claims.Any(c => c.Claim.Type == claim.Type && c.Claim.Value == claim.Value);
    }

    /// <summary>
    /// Checks if the role has a claim by type and value
    /// </summary>
    public bool HasClaim(string claimType, string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimValue))
            return false;

        return _claims.Any(c => c.Claim.Type == claimType && c.Claim.Value == claimValue);
    }

    /// <summary>
    /// Gets all claims of a specific type
    /// </summary>
    public IEnumerable<Claim> GetClaimsByType(string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            return Enumerable.Empty<Claim>();

        return _claims
            .Where(c => c.Claim.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Claim);
    }

    /// <summary>
    /// Gets all permission claims
    /// </summary>
    public IEnumerable<Claim> GetPermissionClaims()
    {
        return GetClaimsByType("permission");
    }

    /// <summary>
    /// Gets all scope claims
    /// </summary>
    public IEnumerable<Claim> GetScopeClaims()
    {
        return GetClaimsByType("scope");
    }

    /// <summary>
    /// Gets all custom claims
    /// </summary>
    public IEnumerable<Claim> GetCustomClaims()
    {
        var standardTypes = new[] { "permission", "role", "scope" };
        return _claims
            .Where(c => !standardTypes.Contains(c.Claim.Type, StringComparer.OrdinalIgnoreCase))
            .Select(c => c.Claim);
    }

    /// <summary>
    /// Clears all claims from the role
    /// </summary>
    public void ClearClaims()
    {
        if (IsSystemRole)
            throw new InvalidOperationException("Cannot clear claims from system role");

        var claimsToRemove = _claims.ToList();
        _claims.Clear();

        foreach (var claim in claimsToRemove)
        {
            AddDomainEvent(new RoleClaimRemovedEvent(Id, Name, claim.Claim.Type, claim.Claim.Value));
        }
    }

    /// <summary>
    /// Gets the total number of users assigned to this role
    /// </summary>
    public int GetUserCount()
    {
        return _userRoles.Count(ur => ur.IsActive);
    }

    /// <summary>
    /// Checks if the role can be deleted
    /// </summary>
    public bool CanBeDeleted()
    {
        return !IsSystemRole && GetUserCount() == 0;
    }
}
