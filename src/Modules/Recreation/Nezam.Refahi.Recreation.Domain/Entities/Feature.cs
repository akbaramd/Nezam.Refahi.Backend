using MCA.SharedKernel.Domain.AggregateRoots;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// FeatureE aggregate root representing extended features for tours
/// </summary>
public sealed class Feature : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? IconClass { get; private set; }
    public Guid? CategoryId { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsRequired { get; private set; }
    public string? DefaultValue { get; private set; }
    public string? ValidationRules { get; private set; }

    // Navigation properties
    public FeatureCategory? Category { get; private set; }


    private readonly List<TourFeature> _tourFeatures = new();
    public IReadOnlyCollection<TourFeature> TourFeatures => _tourFeatures.AsReadOnly();

    // Private constructor for EF Core
    private Feature() : base() { }

    /// <summary>
    /// Creates a new FeatureE
    /// </summary>
    public Feature(
        string name,
        string? description = null,
        string? iconClass = null,
        Guid? categoryId = null,
        int displayOrder = 0,
        bool isRequired = false,
        string? defaultValue = null,
        string? validationRules = null)
        : base(Guid.NewGuid())
    {
        ValidateName(name);

        Name = name.Trim();
        Description = description?.Trim();
        IconClass = iconClass?.Trim();
        CategoryId = categoryId;
        DisplayOrder = displayOrder;
        IsActive = true;
        IsRequired = isRequired;
        DefaultValue = defaultValue?.Trim();
        ValidationRules = validationRules?.Trim();
    }

    /// <summary>
    /// Updates feature details
    /// </summary>
    public void UpdateDetails(string name, string? description = null, string? iconClass = null)
    {
        ValidateName(name);

        Name = name.Trim();
        Description = description?.Trim();
        IconClass = iconClass?.Trim();
    }

    /// <summary>
    /// Sets the feature category
    /// </summary>
    public void SetCategory(Guid? categoryId)
    {
        CategoryId = categoryId;
    }

    /// <summary>
    /// Updates the display order
    /// </summary>
    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Sets whether the feature is required
    /// </summary>
    public void SetRequired(bool isRequired)
    {
        IsRequired = isRequired;
    }

    /// <summary>
    /// Updates the default value
    /// </summary>
    public void SetDefaultValue(string? defaultValue)
    {
        DefaultValue = defaultValue?.Trim();
    }

    /// <summary>
    /// Updates validation rules
    /// </summary>
    public void SetValidationRules(string? validationRules)
    {
        ValidationRules = validationRules?.Trim();
    }

    
  

    /// <summary>
    /// Validates feature against its validation rules
    /// </summary>
    public bool IsValid(string? value = null)
    {
        // If feature is required but no value provided and no default value
        if (IsRequired && string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(DefaultValue))
            return false;

        // Add custom validation logic based on ValidationRules if needed
        if (!string.IsNullOrWhiteSpace(ValidationRules))
        {
            // Implement custom validation logic here
            // This could be extended to support regex patterns, min/max values, etc.
        }

        return true;
    }

    /// <summary>
    /// Gets the effective value (provided value or default value)
    /// </summary>
    public string? GetEffectiveValue(string? providedValue = null)
    {
        return !string.IsNullOrWhiteSpace(providedValue) ? providedValue : DefaultValue;
    }

    /// <summary>
    /// Activates the feature
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the feature
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

  

    // Private validation methods
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Feature name cannot be empty", nameof(name));
        if (name.Length > 200)
            throw new ArgumentException("Feature name cannot exceed 200 characters", nameof(name));
    }
}