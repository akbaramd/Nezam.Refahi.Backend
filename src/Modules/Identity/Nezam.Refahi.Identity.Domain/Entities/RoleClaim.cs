using MCA.SharedKernel.Domain;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Entities;

/// <summary>
/// Represents a claim associated with a role
/// </summary>
public class RoleClaim : Entity<Guid>
{
    public Guid RoleId { get; private set; }
    public Claim Claim { get; private set; } = null!;
    
    // Navigation properties
    public Role Role { get; private set; } = null!;

    // Private constructor for EF Core
    public RoleClaim() : base() { }

    public RoleClaim(Guid roleId, Claim claim) : base(Guid.NewGuid())
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        if (claim == null)
            throw new ArgumentNullException(nameof(claim));

        RoleId = roleId;
        Claim = claim;
    }

    /// <summary>
    /// Updates the claim value
    /// </summary>
    public void UpdateClaim(Claim newClaim)
    {
        if (newClaim == null)
            throw new ArgumentNullException(nameof(newClaim));

        Claim = newClaim;
    }

    /// <summary>
    /// Checks if this claim matches the given type and value
    /// </summary>
    public bool Matches(string claimType, string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimValue))
            return false;

        return Claim.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase) &&
               Claim.Value.Equals(claimValue, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if this claim matches the given claim
    /// </summary>
    public bool Matches(Claim claim)
    {
        if (claim == null)
            return false;

        return Claim.Type == claim.Type && Claim.Value == claim.Value;
    }

    public override string ToString()
    {
        return $"{RoleId}: {Claim}";
    }
}
