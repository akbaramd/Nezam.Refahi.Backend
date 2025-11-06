using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Membership.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Member aggregate root representing a member with multi-role capabilities and one capability containing multiple claims
/// </summary>
public sealed class Member : FullAggregateRoot<Guid>
{
    public Guid ExternalUserId { get; private set; } // Required link to Identity context
    public string MembershipNumber { get; private set; } = null!;
    public NationalId NationalCode { get; private set; } = null!;
    public FullName FullName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public DateTime? BirthDate { get; private set; }
    
    // Special member status for VIP privileges
    public bool IsSpecial { get; private set; }

    private readonly List<MemberRole> _roles = new();
    public IReadOnlyCollection<MemberRole> Roles => _roles.AsReadOnly();

    // Member can have multiple capabilities (many-to-many relationship)
    private readonly List<MemberCapability> _capabilities = new();
    public IReadOnlyCollection<MemberCapability> Capabilities => _capabilities.AsReadOnly();

    // Member can have multiple features (many-to-many relationship)
    private readonly List<MemberFeature> _features = new();
    public IReadOnlyCollection<MemberFeature> Features => _features.AsReadOnly();

    // Member can have access to multiple representative offices (many-to-many relationship)
    private readonly List<MemberAgency> _agencies = new();
    public IReadOnlyCollection<MemberAgency> Agencies => _agencies.AsReadOnly();

    // Private constructor for EF Core
    private Member() : base() { }

    /// <summary>
    /// Creates a new member with required ExternalUserId
    /// </summary>
    public Member(Guid externalUserId, string membershipNumber, NationalId nationalCode, FullName fullName, Email email, PhoneNumber phoneNumber, DateTime? birthDate = null, bool isSpecial = false)
        : base(Guid.NewGuid())
    {
        if (externalUserId == Guid.Empty)
            throw new ArgumentException("External User ID cannot be empty", nameof(externalUserId));
            
        ExternalUserId = externalUserId;
        MembershipNumber = membershipNumber;
        NationalCode = nationalCode ?? throw new ArgumentNullException(nameof(nationalCode));
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        BirthDate = birthDate;
        IsSpecial = isSpecial;
    }

    /// <summary>
    /// Updates the ExternalUserId (should only be called during member creation from user events)
    /// </summary>
    public void UpdateExternalUserId(Guid externalUserId)
    {
        if (externalUserId == Guid.Empty)
            throw new ArgumentException("External User ID cannot be empty", nameof(externalUserId));
            
        ExternalUserId = externalUserId;
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
    public void AssignCapability(string capabilityKey)
    {
        if (string.IsNullOrWhiteSpace(capabilityKey))
            throw new ArgumentException("Capability Key cannot be empty", nameof(capabilityKey));

        // Check if capability is already assigned
        if (_capabilities.Any(c => c.CapabilityKey == capabilityKey))
            return;

        _capabilities.Add(new MemberCapability(Id, capabilityKey));
    }

    /// <summary>
    /// Removes a capability from the member
    /// </summary>
    public void RemoveCapability(string capabilityKey)
    {
        if (string.IsNullOrWhiteSpace(capabilityKey))
            return;

        var capabilitiesToRemove = _capabilities.Where(c => c.CapabilityKey == capabilityKey).ToList();
        foreach (var capability in capabilitiesToRemove)
        {
            _capabilities.Remove(capability);
        }
    }

    /// <summary>
    /// Gets all capabilities for the member
    /// </summary>
    public IEnumerable<MemberCapability> GetValidCapabilities()
    {
        return _capabilities.ToList();
    }

    /// <summary>
    /// Checks if member has a specific capability
    /// </summary>
    public bool HasCapability(string capabilityKey)
    {
        return _capabilities.Any(c => c.CapabilityKey == capabilityKey);
    }

    /// <summary>
    /// Checks if member has a specific capability key
    /// Note: This only checks if member has a capability with the given key
    /// Feature validation should be done through BasicDefinitions context
    /// </summary>
    public bool HasCapabilityKey(string capabilityKey)
    {
        return _capabilities.Any(mc => mc.CapabilityKey == capabilityKey);
    }

    /// <summary>
    /// Assigns a feature to the member
    /// </summary>
    public void AssignFeature(string featureKey)
    {
        if (string.IsNullOrWhiteSpace(featureKey))
            throw new ArgumentException("Feature Key cannot be empty", nameof(featureKey));

        // Check if feature is already assigned
        if (_features.Any(f => f.FeatureKey == featureKey))
            return;

        _features.Add(new MemberFeature(Id, featureKey));
    }

    /// <summary>
    /// Removes a feature from the member
    /// </summary>
    public void RemoveFeature(string featureKey)
    {
        if (string.IsNullOrWhiteSpace(featureKey))
            return;

        var featuresToRemove = _features.Where(f => f.FeatureKey == featureKey).ToList();
        foreach (var feature in featuresToRemove)
        {
            _features.Remove(feature);
        }
    }

    /// <summary>
    /// Gets all features for the member
    /// </summary>
    public IEnumerable<MemberFeature> GetValidFeatures()
    {
        return _features.ToList();
    }

    /// <summary>
    /// Checks if member has a specific feature
    /// </summary>
    public bool HasFeature(string featureKey)
    {
        return _features.Any(f => f.FeatureKey == featureKey);
    }

    /// <summary>
    /// Checks if member has a specific feature key
    /// Note: This only checks if member has a feature with the given key
    /// </summary>
    public bool HasFeatureKey(string featureKey)
    {
        return _features.Any(mf => mf.FeatureKey == featureKey);
    }

    /// <summary>
    /// Assigns access to a representative office
    /// </summary>
    public void AssignOfficeAccess(Guid AgencyId)
    {
        if (AgencyId == Guid.Empty)
            throw new ArgumentException("Representative Office ID cannot be empty", nameof(AgencyId));

        // Check if office access is already assigned
        if (_agencies.Any(a => a.AgencyId == AgencyId))
            return;

        _agencies.Add(new MemberAgency(Id, AgencyId));
    }

    /// <summary>
    /// Removes access to a representative office
    /// </summary>
    public void RemoveOfficeAccess(Guid AgencyId)
    {
        if (AgencyId == Guid.Empty)
            return;

        var agenciesToRemove = _agencies.Where(a => a.AgencyId == AgencyId).ToList();
        foreach (var agency in agenciesToRemove)
        {
            _agencies.Remove(agency);
        }
    }

    /// <summary>
    /// Gets all office accesses for the member
    /// </summary>
    public IEnumerable<MemberAgency> GetValidOfficeAccesses()
    {
        return _agencies.ToList();
    }

    /// <summary>
    /// Checks if member has access to a specific representative office
    /// </summary>
    public bool HasOfficeAccess(Guid AgencyId)
    {
        return _agencies.Any(a => a.AgencyId == AgencyId);
    }

    /// <summary>
    /// Gets all representative office IDs that the member has access to
    /// </summary>
    public IEnumerable<Guid> GetAccessibleOfficeIds()
    {
        return _agencies.Select(a => a.AgencyId);
    }

    /// <summary>
    /// Grants special status to the member
    /// </summary>
    public void GrantSpecialStatus()
    {
        IsSpecial = true;
    }

    /// <summary>
    /// Revokes special status from the member
    /// </summary>
    public void RevokeSpecialStatus()
    {
        IsSpecial = false;
    }

    /// <summary>
    /// Checks if the member has special status
    /// </summary>
    public bool HasSpecialStatus()
    {
        return IsSpecial;
    }

    /// <summary>
    /// Checks if the member can access special tour capacities
    /// </summary>
    public bool CanAccessSpecialCapacities()
    {
        return IsSpecial;
    }

    /// <summary>
    /// Checks if the member can see special tour capacities
    /// </summary>
    public bool CanSeeSpecialCapacities()
    {
        return IsSpecial;
    }
}