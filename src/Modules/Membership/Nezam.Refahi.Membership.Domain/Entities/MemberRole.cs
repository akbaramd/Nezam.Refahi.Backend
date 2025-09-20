using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Represents a domain role assigned to a member
/// Links members to roles with assignment metadata
/// </summary>
public sealed class MemberRole : Entity<Guid>
{
    public Guid MemberId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public string? AssignedBy { get; private set; }           // Who assigned this role
    public string? Notes { get; private set; }               // Assignment notes
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public Member Member { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    // Private constructor for EF Core
    private MemberRole() : base() { }

    public MemberRole(Guid memberId, Guid roleId, DateTime? validFrom = null, DateTime? validTo = null, 
        string? assignedBy = null, string? notes = null)
        : base(Guid.NewGuid())
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        MemberId = memberId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        ValidFrom = validFrom;
        ValidTo = validTo;
        AssignedBy = assignedBy?.Trim();
        Notes = notes?.Trim();
        IsActive = true;
    }

    /// <summary>
    /// Checks if the role assignment is currently valid
    /// </summary>
    public bool IsValid()
    {
        if (!IsActive) return false;
        
        var now = DateTimeOffset.UtcNow;
        
        if (ValidFrom.HasValue && now < ValidFrom.Value)
            return false;
            
        if (ValidTo.HasValue && now > ValidTo.Value)
            return false;
            
        return true;
    }

    /// <summary>
    /// Deactivates the role assignment
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Updates the validity period
    /// </summary>
    public void UpdateValidityPeriod(DateTime? validFrom, DateTime? validTo)
    {
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// Updates the assignment notes
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }
}