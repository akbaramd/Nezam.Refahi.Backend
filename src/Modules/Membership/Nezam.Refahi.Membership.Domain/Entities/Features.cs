using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.AggregateRoots;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Feature catalog aggregate root - configurable by admin
/// Represents different features/capabilities that can be assigned to members
/// </summary>
public sealed class Features : Entity<string>
{
    public string Title { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;

    // Navigation property for many-to-many relationship with Capabilities
    private readonly List<Capability> _capabilities = new();
    public IReadOnlyCollection<Capability> Capabilities => _capabilities.AsReadOnly();

    // Private constructor for EF Core
    private Features() : base() { }

    public Features(string key, string title, string type)
        : base(key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type cannot be empty", nameof(type));

        Title = title.Trim();
        Type = type.Trim();
    }

    /// <summary>
    /// Updates the feature properties
    /// </summary>
    public void Update(string title, string type)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type cannot be empty", nameof(type));

        Title = title.Trim();
        Type = type.Trim();
    }

    #region Feature Types

    public static class FeatureTypes
    {
        public const string ServiceField = "service_field";
        public const string ServiceType = "service_type";
        public const string LicenseStatus = "license_status";
        public const string Grade = "grade";
        public const string SpecialCapability = "special_capability";
    }

    #endregion
}