using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.AggregateRoots;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Entity representing a category for organizing features
/// </summary>
public sealed class FeatureCategory : Entity<Guid>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? IconClass { get; private set; }
    public string? ColorCode { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } 

    // Navigation properties
    private readonly List<Feature> _features= new();
    public IReadOnlyCollection<Feature> Features => _features.AsReadOnly();

    // Private constructor for EF Core
    private FeatureCategory() : base() { }

    /// <summary>
    /// Creates a new feature category
    /// </summary>
    public FeatureCategory(string name, string? description = null, string? iconClass = null,
        string? colorCode = null, int displayOrder = 0)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        IconClass = iconClass?.Trim();
        ColorCode = colorCode?.Trim();
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    /// <summary>
    /// Updates category details
    /// </summary>
    public void UpdateDetails(string name, string? description = null, string? iconClass = null,
        string? colorCode = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        IconClass = iconClass?.Trim();
        ColorCode = colorCode?.Trim();
    }

    /// <summary>
    /// Updates the display order
    /// </summary>
    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Activates the category
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the category
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Gets count of active features in this category
    /// </summary>
    public int GetActiveFeatureCount()
    {
        return _features.Count(f => f.IsActive);
    }
}