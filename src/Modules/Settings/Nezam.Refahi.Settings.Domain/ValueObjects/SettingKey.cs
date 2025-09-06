using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Settings.Domain.ValueObjects;

/// <summary>
/// Represents a unique setting key with validation rules
/// </summary>
public class SettingKey : ValueObject
{
    private readonly string _value;
    
    // Public property to access the value
    public string Value => _value;
    
    public SettingKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Setting key cannot be empty", nameof(value));
            
        if (value.Length > 100)
            throw new ArgumentException("Setting key cannot exceed 100 characters", nameof(value));
            
        if (!IsValidKeyFormat(value))
            throw new ArgumentException("Setting key must contain only letters, numbers, and underscores", nameof(value));
            
        _value = value.ToUpperInvariant();
    }
    
    private static bool IsValidKeyFormat(string key)
    {
        return key.All(c => char.IsLetterOrDigit(c) || c == '_');
    }
    
    public static SettingKey FromString(string value)
    {
        return new SettingKey(value);
    }
    
    public override string ToString() => _value;
    
    public static implicit operator string(SettingKey key) => key._value;
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}
