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
    public Member(Guid externalUserId, string membershipNumber, NationalId nationalCode, FullName fullName, Email email, PhoneNumber phoneNumber, DateTime? birthDate = null)
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
    public void AssignCapability(string capabilityKey, string capabilityTitle, DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(capabilityKey))
            throw new ArgumentException("Capability Key cannot be empty", nameof(capabilityKey));

        // Check if capability is already assigned and active
        if (_capabilities.Any(c => c.CapabilityKey == capabilityKey && c.IsValid()))
            return;

        _capabilities.Add(new MemberCapability(Id, capabilityKey, capabilityTitle, validFrom, validTo, assignedBy, notes));
    }

    /// <summary>
    /// Removes a capability from the member by deactivating it
    /// </summary>
    public void RemoveCapability(string capabilityKey)
    {
        if (string.IsNullOrWhiteSpace(capabilityKey))
            return;

        var activeCapabilities = _capabilities.Where(c => c.CapabilityKey == capabilityKey && c.IsActive).ToList();
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
    public bool HasCapability(string capabilityKey)
    {
        return _capabilities.Any(c => c.CapabilityKey == capabilityKey && c.IsValid());
    }




    /// <summary>
    /// Checks if member has a specific capability key
    /// Note: This only checks if member has a capability with the given key
    /// Feature validation should be done through BasicDefinitions context
    /// </summary>
    public bool HasCapabilityKey(string capabilityKey)
    {
        return GetValidCapabilities()
            .Any(mc => mc.CapabilityKey == capabilityKey);
    }

    /// <summary>
    /// Assigns a feature to the member
    /// </summary>
    public void AssignFeature(string featureKey, string featureTitle, DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(featureKey))
            throw new ArgumentException("Feature Key cannot be empty", nameof(featureKey));

        // Check if feature is already assigned and active
        if (_features.Any(f => f.FeatureKey == featureKey && f.IsValid()))
            return;

        _features.Add(new MemberFeature(Id, featureKey, featureTitle, validFrom, validTo, assignedBy, notes));
    }

    /// <summary>
    /// Removes a feature from the member by deactivating it
    /// </summary>
    public void RemoveFeature(string featureKey)
    {
        if (string.IsNullOrWhiteSpace(featureKey))
            return;

        var activeFeatures = _features.Where(f => f.FeatureKey == featureKey && f.IsActive).ToList();
        foreach (var feature in activeFeatures)
        {
            feature.Deactivate();
        }
    }

    /// <summary>
    /// Gets all valid (active and within validity period) features for the member
    /// </summary>
    public IEnumerable<MemberFeature> GetValidFeatures()
    {
        return _features.Where(f => f.IsValid()).ToList();
    }

    /// <summary>
    /// Checks if member has a specific feature
    /// </summary>
    public bool HasFeature(string featureKey)
    {
        return _features.Any(f => f.FeatureKey == featureKey && f.IsValid());
    }

    /// <summary>
    /// Checks if member has a specific feature key
    /// Note: This only checks if member has a feature with the given key
    /// </summary>
    public bool HasFeatureKey(string featureKey)
    {
        return GetValidFeatures()
            .Any(mf => mf.FeatureKey == featureKey);
    }

    /// <summary>
    /// Gets features that are expiring soon
    /// </summary>
    public IEnumerable<MemberFeature> GetExpiringFeatures(TimeSpan timeThreshold)
    {
        var cutoffTime = DateTimeOffset.UtcNow.Add(timeThreshold);
        return _features.Where(f =>
            f.IsActive &&
            f.ValidTo.HasValue &&
            f.ValidTo.Value <= cutoffTime &&
            !f.IsExpired()).ToList();
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
    public void AssignOfficeAccess(Guid AgencyId, string officeCode, string officeTitle,
        DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null, string? accessLevel = null)
    {
        if (AgencyId == Guid.Empty)
            throw new ArgumentException("Representative Office ID cannot be empty", nameof(AgencyId));

        // Check if office access is already assigned and active
        if (_agencies.Any(a => a.AgencyId == AgencyId && a.IsValid()))
            return;

        _agencies.Add(new MemberAgency(Id, AgencyId, officeCode, officeTitle, validFrom, validTo, assignedBy, notes, accessLevel));
    }

    /// <summary>
    /// Removes access to a representative office by deactivating it
    /// </summary>
    public void RemoveOfficeAccess(Guid AgencyId)
    {
        if (AgencyId == Guid.Empty)
            return;

        var activeAgencies = _agencies.Where(a => a.AgencyId == AgencyId && a.IsActive).ToList();
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
    public bool HasOfficeAccess(Guid AgencyId)
    {
        return _agencies.Any(a => a.AgencyId == AgencyId && a.IsValid());
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
        return GetValidOfficeAccesses().Select(a => a.AgencyId);
    }

    /// <summary>
    /// Checks if member has full access to a specific representative office
    /// </summary>
    public bool HasFullOfficeAccess(Guid AgencyId)
    {
        return _agencies.Any(a => 
            a.AgencyId == AgencyId && 
            a.IsValid() && 
            a.HasFullAccess());
    }

    /// <summary>
    /// Checks if member has read-only access to a specific representative office
    /// </summary>
    public bool HasReadOnlyOfficeAccess(Guid AgencyId)
    {
        return _agencies.Any(a => 
            a.AgencyId == AgencyId && 
            a.IsValid() && 
            a.HasReadOnlyAccess());
    }
}