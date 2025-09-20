using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Membership.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Member aggregate root representing a member with multi-role capabilities and one capability containing multiple claims
/// </summary>
public sealed class Member : FullAggregateRoot<Guid>
{
    public Guid? UserId { get; private set; } // Optional link to Auth module
    public string MembershipNumber { get; private set; } = null!;
    public NationalId NationalCode { get; private set; } = null!;
    public FullName FullName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public DateTime? BirthDate { get; private set; }

    private readonly List<MemberRole> _roles = new();
    public IReadOnlyCollection<MemberRole> Roles => _roles.AsReadOnly();

    // Member can have multiple capabilities (many-to-many relationship)
    private readonly List<MemberCapability> _capabilities = new();
    public IReadOnlyCollection<MemberCapability> Capabilities => _capabilities.AsReadOnly();

    // Private constructor for EF Core
    private Member() : base() { }

    /// <summary>
    /// Creates a new member
    /// </summary>
    public Member(string membershipNumber,NationalId nationalCode, FullName fullName, Email email, PhoneNumber phoneNumber, DateTime? birthDate = null)
        : base(Guid.NewGuid())
    {
      MembershipNumber = membershipNumber;
        NationalCode = nationalCode ?? throw new ArgumentNullException(nameof(nationalCode));
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        BirthDate = birthDate;
    }

    /// <summary>
    /// Sets the optional UserId for linking to Auth module
    /// </summary>
    public void SetUserId(Guid? userId)
    {
        UserId = userId;
    }

    /// <summary>
    /// Assigns a domain role to the member
    /// </summary>
    public void AssignRole(Guid roleId, DateTime? validFrom = null, DateTime? validTo = null, 
        string? assignedBy = null, string? notes = null)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        // Check if role is already assigned and active
        if (_roles.Any(r => r.RoleId == roleId && r.IsValid())) return;
        
        _roles.Add(new MemberRole(Id, roleId, validFrom, validTo, assignedBy, notes));
    }

    /// <summary>
    /// Removes a domain role from the member by deactivating it
    /// </summary>
    public void RemoveRole(Guid roleId)
    {
        if (roleId == Guid.Empty) return;
        
        var activeRoles = _roles.Where(r => r.RoleId == roleId && r.IsActive).ToList();
        foreach (var role in activeRoles)
        {
            role.Deactivate();
        }
    }

    /// <summary>
    /// Gets all valid (active and within validity period) roles for the member
    /// </summary>
    public IEnumerable<MemberRole> GetValidRoles()
    {
        return _roles.Where(r => r.IsValid()).ToList();
    }

    /// <summary>
    /// Checks if member has a specific role
    /// </summary>
    public bool HasRole(Guid roleId)
    {
        return _roles.Any(r => r.RoleId == roleId && r.IsValid());
    }

    /// <summary>
    /// Assigns a capability to the member
    /// </summary>
    public void AssignCapability(string capabilityId, DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null)
    {
        if (capabilityId == string.Empty)
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        // Check if capability is already assigned and active
        if (_capabilities.Any(c => c.CapabilityId == capabilityId && c.IsValid()))
            return;

        _capabilities.Add(new MemberCapability(Id, capabilityId, validFrom, validTo, assignedBy, notes));
    }

    /// <summary>
    /// Removes a capability from the member by deactivating it
    /// </summary>
    public void RemoveCapability(string capabilityId)
    {
        if (capabilityId == string.Empty)
            return;

        var activeCapabilities = _capabilities.Where(c => c.CapabilityId == capabilityId && c.IsActive).ToList();
        foreach (var capability in activeCapabilities)
        {
            capability.Deactivate();
        }
    }

    /// <summary>
    /// Gets all valid (active and within validity period) capabilities for the member
    /// </summary>
    public IEnumerable<MemberCapability> GetValidCapabilities()
    {
        return _capabilities.Where(c => c.IsValid()).ToList();
    }

    /// <summary>
    /// Checks if member has a specific capability
    /// </summary>
    public bool HasCapability(string capabilityId)
    {
        return _capabilities.Any(c => c.CapabilityId == capabilityId && c.IsValid());
    }




    /// <summary>
    /// Checks if member has access to a specific claim type through any capability
    /// </summary>
    public bool HasClaimType(string featureId)
    {
        return GetValidCapabilities()
            .Any(mc => mc.Capability?.HasFeature(featureId) ?? false);
    }

    /// <summary>
    /// Gets capabilities that are expiring soon
    /// </summary>
    public IEnumerable<MemberCapability> GetExpiringCapabilities(TimeSpan timeThreshold)
    {
        var cutoffTime = DateTimeOffset.UtcNow.Add(timeThreshold);
        return _capabilities.Where(c =>
            c.IsActive &&
            c.ValidTo.HasValue &&
            c.ValidTo.Value <= cutoffTime &&
            !c.IsExpired()).ToList();
    }


}