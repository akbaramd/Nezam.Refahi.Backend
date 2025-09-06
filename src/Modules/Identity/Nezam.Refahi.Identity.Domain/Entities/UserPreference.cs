using MCA.SharedKernel.Domain;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Services;

namespace Nezam.Refahi.Identity.Domain.Entities;

/// <summary>
/// Represents a user preference with type-safe value storage
/// </summary>
public class UserPreference : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public PreferenceKey Key { get; private set; } = null!;
    public PreferenceValue Value { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public PreferenceCategory Category { get; private set; }
    
    // Navigation properties
    public User User { get; private set; } = null!;
    
    // Private constructor for EF Core
    public UserPreference() : base() { }
    
    public UserPreference(Guid userId, PreferenceKey key, PreferenceValue value, string description, int displayOrder = 0, PreferenceCategory category = PreferenceCategory.General)
        : base(Guid.NewGuid())
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
        if (key == null)
            throw new ArgumentNullException(nameof(key));
            
        if (value == null)
            throw new ArgumentNullException(nameof(value));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));
            
        if (description.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters", nameof(description));
            
        UserId = userId;
        Key = key;
        Value = value;
        Description = description;
        DisplayOrder = displayOrder;
        Category = category;
        IsActive = true;
    }
    
    /// <summary>
    /// Updates the preference value
    /// </summary>
    public void UpdateValue(PreferenceValue newValue)
    {
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));
            
        if (!IsActive)
            throw new InvalidOperationException("Cannot update inactive preference");
            
        // Validate that the new value type matches the current preference type
        if (Value.Type != newValue.Type)
            throw new InvalidOperationException($"Preference type mismatch. Expected: {Value.Type}, Got: {newValue.Type}");
            
        Value = newValue;
    }
    
    /// <summary>
    /// Updates the preference value using a string and automatically determines the type
    /// </summary>
    public void UpdateValue(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException("Value cannot be empty", nameof(newValue));
            
        // Create new PreferenceValue with the same type as current
        var newPreferenceValue = new PreferenceValue(newValue, Value.Type);
        
        UpdateValue(newPreferenceValue);
    }
    
    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));
            
        if (description.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters", nameof(description));
            
        Description = description;
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
    /// Gets the typed value based on the preference type
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
            
        throw new InvalidOperationException($"Cannot convert preference value to type {typeof(T).Name}");
    }
    
    /// <summary>
    /// Checks if this preference has a specific value
    /// </summary>
    public bool HasValue(string value)
    {
        return Value.RawValue.Equals(value, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Checks if this preference has a specific typed value
    /// </summary>
    public bool HasValue<T>(T value)
    {
        try
        {
            var currentValue = GetTypedValue<T>();
            return EqualityComparer<T>.Default.Equals(currentValue, value);
        }
        catch
        {
            return false;
        }
    }
}
