using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Identity.Domain.ValueObjects;

/// <summary>
/// Represents a unique preference key with validation rules
/// </summary>
public class PreferenceKey : ValueObject
{
    private readonly string _value;
    
    // Public property to access the value
    public string Value => _value;
    
    public PreferenceKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Preference key cannot be empty", nameof(value));
            
        if (value.Length > 100)
            throw new ArgumentException("Preference key cannot exceed 100 characters", nameof(value));
            
        if (!IsValidKeyFormat(value))
            throw new ArgumentException("Preference key must contain only letters, numbers, and underscores", nameof(value));
            
        _value = value.ToUpperInvariant();
    }
    
    private static bool IsValidKeyFormat(string key)
    {
        return key.All(c => char.IsLetterOrDigit(c) || c == '_');
    }
    
    public static PreferenceKey FromString(string value)
    {
        return new PreferenceKey(value);
    }
    
    public override string ToString() => _value;
    
    public static implicit operator string(PreferenceKey key) => key._value;
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}
