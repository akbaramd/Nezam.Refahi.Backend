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

    // Member can have access to multiple representative offices (many-to-many relationship)
    private readonly List<MemberAgency> _agencies = new();
    public IReadOnlyCollection<MemberAgency> Agencies => _agencies.AsReadOnly();

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
    public void AssignCapability(string capabilityId, string capabilityName, DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null)
    {
        if (capabilityId == string.Empty)
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        // Check if capability is already assigned and active
        if (_capabilities.Any(c => c.CapabilityId == capabilityId && c.IsValid()))
            return;

        _capabilities.Add(new MemberCapability(Id, capabilityId, capabilityName, validFrom, validTo, assignedBy, notes));
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
    /// Checks if member has a specific capability key
    /// Note: This only checks if member has a capability with the given key
    /// Feature validation should be done through BasicDefinitions context
    /// </summary>
    public bool HasCapabilityKey(string capabilityKey)
    {
        return GetValidCapabilities()
            .Any(mc => mc.CapabilityId == capabilityKey);
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

    /// <summary>
    /// Assigns access to a representative office
    /// </summary>
    public void AssignOfficeAccess(Guid representativeOfficeId, string officeCode, string officeTitle,
        DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null, string? accessLevel = null)
    {
        if (representativeOfficeId == Guid.Empty)
            throw new ArgumentException("Representative Office ID cannot be empty", nameof(representativeOfficeId));

        // Check if office access is already assigned and active
        if (_agencies.Any(a => a.RepresentativeOfficeId == representativeOfficeId && a.IsValid()))
            return;

        _agencies.Add(new MemberAgency(Id, representativeOfficeId, officeCode, officeTitle, validFrom, validTo, assignedBy, notes, accessLevel));
    }

    /// <summary>
    /// Removes access to a representative office by deactivating it
    /// </summary>
    public void RemoveOfficeAccess(Guid representativeOfficeId)
    {
        if (representativeOfficeId == Guid.Empty)
            return;

        var activeAgencies = _agencies.Where(a => a.RepresentativeOfficeId == representativeOfficeId && a.IsActive).ToList();
        foreach (var agency in activeAgencies)
        {
            agency.Deactivate();
        }
    }

    /// <summary>
    /// Gets all valid (active and within validity period) office accesses for the member
    /// </summary>
    public IEnumerable<MemberAgency> GetValidOfficeAccesses()
    {
        return _agencies.Where(a => a.IsValid()).ToList();
    }

    /// <summary>
    /// Checks if member has access to a specific representative office
    /// </summary>
    public bool HasOfficeAccess(Guid representativeOfficeId)
    {
        return _agencies.Any(a => a.RepresentativeOfficeId == representativeOfficeId && a.IsValid());
    }

    /// <summary>
    /// Gets office accesses that are expiring soon
    /// </summary>
    public IEnumerable<MemberAgency> GetExpiringOfficeAccesses(TimeSpan timeThreshold)
    {
        var cutoffTime = DateTimeOffset.UtcNow.Add(timeThreshold);
        return _agencies.Where(a =>
            a.IsActive &&
            a.ValidTo.HasValue &&
            a.ValidTo.Value <= cutoffTime &&
            !a.IsExpired()).ToList();
    }

    /// <summary>
    /// Gets all representative office IDs that the member has access to
    /// </summary>
    public IEnumerable<Guid> GetAccessibleOfficeIds()
    {
        return GetValidOfficeAccesses().Select(a => a.RepresentativeOfficeId);
    }

    /// <summary>
    /// Checks if member has full access to a specific representative office
    /// </summary>
    public bool HasFullOfficeAccess(Guid representativeOfficeId)
    {
        return _agencies.Any(a => 
            a.RepresentativeOfficeId == representativeOfficeId && 
            a.IsValid() && 
            a.HasFullAccess());
    }

    /// <summary>
    /// Checks if member has read-only access to a specific representative office
    /// </summary>
    public bool HasReadOnlyOfficeAccess(Guid representativeOfficeId)
    {
        return _agencies.Any(a => 
            a.RepresentativeOfficeId == representativeOfficeId && 
            a.IsValid() && 
            a.HasReadOnlyAccess());
    }
}