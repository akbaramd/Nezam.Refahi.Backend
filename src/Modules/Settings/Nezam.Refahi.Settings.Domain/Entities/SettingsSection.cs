using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Settings.Domain.Entities;

/// <summary>
/// Represents a logical section for grouping related settings
/// </summary>
public class SettingsSection : Entity<Guid>
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    
    // Navigation properties
    public virtual ICollection<SettingsCategory> Categories { get; private set; } = new List<SettingsCategory>();
    
    // Private constructor for EF Core
    public SettingsSection() : base() { }
    
    public SettingsSection(string name, string description, int displayOrder = 0)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Section name cannot be empty", nameof(name));
            
        if (name.Length > 100)
            throw new ArgumentException("Section name cannot exceed 100 characters", nameof(name));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Section description cannot be empty", nameof(description));
            
        if (description.Length > 500)
            throw new ArgumentException("Section description cannot exceed 500 characters", nameof(description));
            
        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
    
    public void UpdateDetails(string name, string description, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Section name cannot be empty", nameof(name));
            
        if (name.Length > 100)
            throw new ArgumentException("Section name cannot exceed 100 characters", nameof(name));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Section description cannot be empty", nameof(description));
            
        if (description.Length > 500)
            throw new ArgumentException("Section description cannot exceed 500 characters", nameof(description));
            
        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
    }
    
    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
        }
    }
    
    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
        }
    }
    
    public void AddCategory(SettingsCategory category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));
            
        if (Categories.Any(c => c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Category '{category.Name}' already exists in section '{Name}'");
            
        Categories.Add(category);
    }
    
    public void RemoveCategory(SettingsCategory category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));
            
        Categories.Remove(category);
    }
}
