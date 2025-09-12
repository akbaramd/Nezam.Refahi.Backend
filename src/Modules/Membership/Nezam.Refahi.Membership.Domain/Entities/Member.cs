using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Membership.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Member aggregate root representing a member with multi-role and multi-claim capabilities
/// </summary>
public sealed class Member : FullyAuditableAggregateRoot<Guid>
{
    public Guid? UserId { get; private set; } // Optional link to Auth module
    public string MembershipNumber { get; private set; } = null!;
    public NationalId NationalCode { get; private set; } = null!;
    public FullName FullName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public DateOnly? BirthDate { get; private set; }

    private readonly List<MemberRole> _roles = new();
    public IReadOnlyCollection<MemberRole> Roles => _roles.AsReadOnly();

    private readonly List<MemberClaim> _claims = new();
    public IReadOnlyCollection<MemberClaim> Claims => _claims.AsReadOnly();

    // Private constructor for EF Core
    private Member() : base() { }

    /// <summary>
    /// Creates a new member
    /// </summary>
    public Member(string membershipNumber,NationalId nationalCode, FullName fullName, Email email, PhoneNumber phoneNumber, DateOnly? birthDate = null)
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
    /// Adds a claim with provenance and validity information
    /// </summary>
    public void AddClaim(Guid claimTypeId, string value, 
      DateTime? validFrom = null, DateTime? validTo = null, 
        bool isSensitive = false, bool isSystemManaged = false)
    {
        if (claimTypeId == Guid.Empty)
            throw new ArgumentException("Claim type ID cannot be empty", nameof(claimTypeId));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Claim value cannot be empty", nameof(value));
       
        // Check if same claim already exists
        var existingClaim = _claims.FirstOrDefault(c => 
            c.ClaimTypeId == claimTypeId && 
            c.Value == value && 
            !c.IsExpired());
        
        if (existingClaim != null)
            return;

        var memberClaim = new MemberClaim(Id, claimTypeId, value, 
            validFrom, validTo, isSensitive, isSystemManaged);
        _claims.Add(memberClaim);
    }

    /// <summary>
    /// Removes a claim by type and value
    /// </summary>
    public void RemoveClaim(Guid claimTypeId, string value)
    {
        if (claimTypeId == Guid.Empty || string.IsNullOrWhiteSpace(value))
            return;

        var claimsToRemove = _claims.Where(c => 
            c.ClaimTypeId == claimTypeId && 
            c.Value == value && 
            !c.IsExpired()).ToList();

        foreach (var claim in claimsToRemove)
        {
            _claims.Remove(claim);
        }
    }

    /// <summary>
    /// Gets all valid (non-expired) claims
    /// </summary>
    public IEnumerable<MemberClaim> GetValidClaims()
    {
        return _claims.Where(c => !c.IsExpired()).ToList();
    }

    /// <summary>
    /// Gets claims by type
    /// </summary>
    public IEnumerable<MemberClaim> GetClaimsByType(Guid claimTypeId)
    {
        return _claims.Where(c => c.ClaimTypeId == claimTypeId && !c.IsExpired()).ToList();
    }

    /// <summary>
    /// Checks if member has a specific claim
    /// </summary>
    public bool HasClaim(Guid claimTypeId, string value)
    {
        return _claims.Any(c => 
            c.ClaimTypeId == claimTypeId && 
            c.Value == value && 
            !c.IsExpired());
    }

    /// <summary>
    /// Gets claims that are expiring soon
    /// </summary>
    public IEnumerable<MemberClaim> GetExpiringClaims(TimeSpan timeThreshold)
    {
        var cutoffTime = DateTimeOffset.UtcNow.Add(timeThreshold);
        return _claims.Where(c => 
            c.ValidTo.HasValue && 
            c.ValidTo.Value <= cutoffTime && 
            !c.IsExpired()).ToList();
    }

}