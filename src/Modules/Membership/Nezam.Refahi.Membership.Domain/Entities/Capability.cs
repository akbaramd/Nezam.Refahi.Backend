using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.AggregateRoots;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Capability aggregate root representing a collection of claims that define what a member can do
/// Each member has one capability that groups all their claims together
/// </summary>
public sealed class Capability : Entity<string>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }

    private readonly List<Features> _features = new();
    public IReadOnlyCollection<Features> Features => _features.AsReadOnly();

    // Private constructor for EF Core
    private Capability() : base() { }

    /// <summary>
    /// Creates a new capability
    /// </summary>
    public Capability(string key, string name, string description, DateTime? validFrom = null, DateTime? validTo = null)
        : base(key)
    {
      if (string.IsNullOrWhiteSpace(key))
        throw new ArgumentException("Key cannot be empty", nameof(key));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Name = name.Trim();
        Description = description.Trim();
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// Updates the capability information
    /// </summary>
    public void Update(string name, string description, DateTime? validFrom = null, DateTime? validTo = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Name = name.Trim();
        Description = description.Trim();
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// Adds a claim type to the capability
    /// </summary>
    public void AddFeature(Features claim)
    {
        if (claim == null)
            throw new ArgumentNullException(nameof(claim));

        // Check if claim type already exists in this capability
        var existingClaimType = _features.FirstOrDefault(ct => ct.Id == claim.Id);
        if (existingClaimType != null)
            return;

        _features.Add(claim);
    }

    /// <summary>
    /// Removes a claim type from the capability
    /// </summary>
    public void RemoveFeature(string featureId)
    {
        if (featureId == string.Empty)
            return;

        var claimTypeToRemove = _features.FirstOrDefault(ct => ct.Id == featureId);
        if (claimTypeToRemove != null)
        {
            _features.Remove(claimTypeToRemove);
        }
    }

    /// <summary>
    /// Gets all claim types in the capability
    /// </summary>
    public IEnumerable<Features> GetClaimTypes()
    {
        return _features.ToList();
    }



    /// <summary>
    /// Checks if capability has a specific claim type
    /// </summary>
    public bool HasFeature(string featureId)
    {
        return _features.Any(ct => ct.Id == featureId);
    }


    /// <summary>
    /// Activates the capability
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the capability
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Checks if the capability is currently valid (active and within validity period)
    /// </summary>
    public bool IsValid()
    {
        if (!IsActive)
            return false;

        var now = DateTimeOffset.UtcNow;

        if (ValidFrom.HasValue && now < ValidFrom.Value)
            return false;

        if (ValidTo.HasValue && now > ValidTo.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the capability is expired
    /// </summary>
    public bool IsExpired()
    {
        return ValidTo.HasValue && DateTimeOffset.UtcNow > ValidTo.Value;
    }

    public override string ToString()
    {
      return $"{Id}: {Name}";
    }
}