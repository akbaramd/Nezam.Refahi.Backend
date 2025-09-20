using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Domain.Entities;

/// <summary>
/// Represents an individual system setting with event sourcing capabilities
/// </summary>
public class Setting : FullAggregateRoot<Guid>
{
    public SettingKey Key { get; private set; } = null!;
    public SettingValue Value { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsReadOnly { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    
    // Foreign keys
    public Guid CategoryId { get; private set; }
    
    // Navigation properties
    public virtual SettingsCategory Category { get; private set; } = null!;
    public virtual ICollection<SettingChangeEvent> ChangeEvents { get; private set; } = new List<SettingChangeEvent>();
    
    // Private constructor for EF Core
    public Setting() : base() { }
    
    public Setting(SettingKey key, SettingValue value, string description, Guid categoryId, bool isReadOnly = false, int displayOrder = 0)
        : base(Guid.NewGuid())
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
            
        if (value == null)
            throw new ArgumentNullException(nameof(value));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));
            
        if (description.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters", nameof(description));
            
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category ID cannot be empty", nameof(categoryId));
            
        Key = key;
        Value = value;
        Description = description;
        CategoryId = categoryId;
        IsReadOnly = isReadOnly;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
    
    /// <summary>
    /// Updates the setting value and creates a change event for audit trail
    /// </summary>
    public void UpdateValue(SettingValue newValue, Guid userId, string? changeReason = null)
    {
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));
            
        if (IsReadOnly)
            throw new InvalidOperationException("Cannot update read-only setting");
            
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
        // Create change event before updating
        var changeEvent = new SettingChangeEvent(
            Id,
            Key,
            Value,
            newValue,
            userId,
            changeReason
        );
        
        ChangeEvents.Add(changeEvent);
        
        // Update the value
        Value = newValue;
    }
    
    /// <summary>
    /// Updates the setting value using a string and automatically determines the type
    /// </summary>
    public void UpdateValue(string newValue, Guid userId, string? changeReason = null)
    {
        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException("Value cannot be empty", nameof(newValue));
            
        // Create new SettingValue with the same type as current
        var newSettingValue = new SettingValue(newValue, Value.Type);
        
        UpdateValue(newSettingValue, userId, changeReason);
    }
    
    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));
            
        if (description.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters", nameof(description));
            
        Description = description;
    }
    
    public void SetReadOnly(bool isReadOnly)
    {
        IsReadOnly = isReadOnly;
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
    
    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
    
    /// <summary>
    /// Gets the typed value based on the setting type
    /// </summary>
    public T GetTypedValue<T>()
    {
        if (typeof(T) == typeof(string))
            return (T)(object)Value.AsString();
        if (typeof(T) == typeof(int))
            return (T)(object)Value.AsInteger();
        if (typeof(T) == typeof(bool))
            return (T)(object)Value.AsBoolean();
        if (typeof(T) == typeof(decimal))
            return (T)(object)Value.AsDecimal();
        if (typeof(T) == typeof(DateTime))
            return (T)(object)Value.AsDateTime();
        if (typeof(T) == typeof(object))
            return (T)(object)Value.AsJson<object>()!;
            
        throw new InvalidOperationException($"Cannot convert setting value to type {typeof(T).Name}");
    }
    
    /// <summary>
    /// Gets the last change event for this setting
    /// </summary>
    public SettingChangeEvent? GetLastChangeEvent()
    {
        return ChangeEvents.OrderByDescending(e => e.CreatedAt).FirstOrDefault();
    }
}
