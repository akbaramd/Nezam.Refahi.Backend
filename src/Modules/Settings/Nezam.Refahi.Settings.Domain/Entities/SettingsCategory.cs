using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Settings.Domain.Entities;

/// <summary>
/// Represents a category within a settings section for further grouping
/// </summary>
public class SettingsCategory : Entity<Guid>
{
 
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    
    // Foreign key
    public Guid SectionId { get; private set; }
    
    // Navigation properties
    public virtual SettingsSection Section { get; private set; } = null!;
    public virtual ICollection<Setting> Settings { get; private set; } = new List<Setting>();
    
    // Private constructor for EF Core
    public SettingsCategory() : base() { }
    
    public SettingsCategory(string name, string description, Guid sectionId, int displayOrder = 0)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));
            
        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters", nameof(name));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Category description cannot be empty", nameof(description));
            
        if (description.Length > 500)
            throw new ArgumentException("Category description cannot exceed 500 characters", nameof(description));
            
        if (sectionId == Guid.Empty)
            throw new ArgumentException("Section ID cannot be empty", nameof(sectionId));
            
        Name = name;
        Description = description;
        SectionId = sectionId;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
    
    public void UpdateDetails(string name, string description, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));
            
        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters", nameof(name));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Category description cannot be empty", nameof(description));
            
        if (description.Length > 500)
            throw new ArgumentException("Category description cannot exceed 500 characters", nameof(description));
            
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
    
    public void AddSetting(Setting setting)
    {
        if (setting == null)
            throw new ArgumentNullException(nameof(setting));
            
        if (Settings.Any(s => s.Key.Value.Equals(setting.Key.Value, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Setting '{setting.Key.Value}' already exists in category '{Name}'");
            
        Settings.Add(setting);
    }
    
    public void RemoveSetting(Setting setting)
    {
        if (setting == null)
            throw new ArgumentNullException(nameof(setting));
            
        Settings.Remove(setting);
    }
}
