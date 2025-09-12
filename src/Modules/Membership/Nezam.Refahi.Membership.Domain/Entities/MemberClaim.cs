using MCA.SharedKernel.Domain;
using Nezam.Refahi.Membership.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Represents a claim value assigned to a member with provenance and validity information
/// Each value is stored as a separate record (multi-value = multiple records)
/// </summary>
public sealed class MemberClaim : Entity<Guid>
{
    public Guid MemberId { get; private set; }
    public Guid ClaimTypeId { get; private set; }
    public string Value { get; private set; } = string.Empty;

    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public bool IsSensitive { get; private set; }             // Requires encryption/masking
    public bool IsSystemManaged { get; private set; }         // Locked and not manually editable

    // Navigation properties
    public Member Member { get; private set; } = null!;
    public ClaimType ClaimType { get; private set; } = null!;

    // Private constructor for EF Core
    private MemberClaim() : base() { }

    public MemberClaim(Guid memberId, Guid claimTypeId, string value, 
      DateTime? validFrom = null, DateTime? validTo = null, 
        bool isSensitive = false, bool isSystemManaged = false)
        : base(Guid.NewGuid())
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
        if (claimTypeId == Guid.Empty)
            throw new ArgumentException("Claim type ID cannot be empty", nameof(claimTypeId));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty", nameof(value));

        MemberId = memberId;
        ClaimTypeId = claimTypeId;
        Value = value.Trim();
        ValidFrom = validFrom;
        ValidTo = validTo;
        IsSensitive = isSensitive;
        IsSystemManaged = isSystemManaged;
    }

    /// <summary>
    /// Checks if the claim is currently valid (within validity period)
    /// </summary>
    public bool IsValid()
    {
        var now = DateTimeOffset.UtcNow;
        
        if (ValidFrom.HasValue && now < ValidFrom.Value)
            return false;
            
        if (ValidTo.HasValue && now > ValidTo.Value)
            return false;
            
        return true;
    }

    /// <summary>
    /// Checks if the claim is expired
    /// </summary>
    public bool IsExpired()
    {
        return ValidTo.HasValue && DateTimeOffset.UtcNow > ValidTo.Value;
    }

    /// <summary>
    /// Gets the remaining time until expiration
    /// </summary>
    public TimeSpan? GetTimeUntilExpiration()
    {
        if (!ValidTo.HasValue)
            return null;

        var timeRemaining = ValidTo.Value - DateTimeOffset.UtcNow;
        return timeRemaining > TimeSpan.Zero ? timeRemaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Updates the validity period (only if not system managed)
    /// </summary>
    public void UpdateValidityPeriod(DateTime? validFrom, DateTime? validTo)
    {
        if (IsSystemManaged)
            throw new InvalidOperationException("Cannot update validity period of system managed claims");

        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// Updates the claim value (only if not system managed)
    /// </summary>
    public void UpdateValue(string value)
    {
        if (IsSystemManaged)
            throw new InvalidOperationException("Cannot update value of system managed claims");
        
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty", nameof(value));

        Value = value.Trim();
    }
}