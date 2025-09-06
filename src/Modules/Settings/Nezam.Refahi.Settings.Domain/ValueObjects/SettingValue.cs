using System.Text.Json;
using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Settings.Domain.ValueObjects;

/// <summary>
/// Represents a setting value with type validation and conversion capabilities
/// </summary>
public class SettingValue : ValueObject
{
    public string RawValue { get; private set; } = null!;
    public SettingType Type { get; private set; }
    
    // Private parameterless constructor for EF Core
    private SettingValue() { }
    
    public SettingValue(string value, SettingType type)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
            
        if (string.IsNullOrWhiteSpace(value) && type != SettingType.String)
            throw new ArgumentException("Value cannot be empty for non-string types", nameof(value));
            
        RawValue = value;
        Type = type;
        
        // Validate the value based on type
        ValidateValue(value, type);
    }
    
    private static void ValidateValue(string value, SettingType type)
    {
        switch (type)
        {
            case SettingType.Integer:
                if (!int.TryParse(value, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid integer", nameof(value));
                break;
                
            case SettingType.Boolean:
                if (!bool.TryParse(value, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid boolean", nameof(value));
                break;
                
            case SettingType.Decimal:
                if (!decimal.TryParse(value, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid decimal", nameof(value));
                break;
                
            case SettingType.DateTime:
                if (!DateTime.TryParse(value, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid datetime", nameof(value));
                break;
                
            case SettingType.Json:
                try
                {
                    JsonDocument.Parse(value);
                }
                catch (JsonException)
                {
                    throw new ArgumentException($"Value '{value}' is not valid JSON", nameof(value));
                }
                break;
                
            case SettingType.Email:
                if (!IsValidEmail(value))
                    throw new ArgumentException($"Value '{value}' is not a valid email address", nameof(value));
                break;
                
            case SettingType.Url:
                if (!Uri.TryCreate(value, UriKind.Absolute, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid URL", nameof(value));
                break;
                
            case SettingType.PhoneNumber:
                if (!IsValidPhoneNumber(value))
                    throw new ArgumentException($"Value '{value}' is not a valid phone number", nameof(value));
                break;
        }
    }
    
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    private static bool IsValidPhoneNumber(string phone)
    {
        // Basic phone number validation - can be enhanced based on requirements
        return phone.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ' || c == '(' || c == ')');
    }
    
    // Type conversion methods
    public string AsString() => RawValue;
    
    public int AsInteger() => int.Parse(RawValue);
    
    public bool AsBoolean() => bool.Parse(RawValue);
    
    public decimal AsDecimal() => decimal.Parse(RawValue);
    
    public DateTime AsDateTime() => DateTime.Parse(RawValue);
    
    public T? AsJson<T>() => JsonSerializer.Deserialize<T>(RawValue);
    
    /// <summary>
    /// Gets the typed value based on the setting type
    /// </summary>
    public T GetTypedValue<T>()
    {
        if (typeof(T) == typeof(string))
            return (T)(object)AsString();
        if (typeof(T) == typeof(int))
            return (T)(object)AsInteger();
        if (typeof(T) == typeof(bool))
            return (T)(object)AsBoolean();
        if (typeof(T) == typeof(decimal))
            return (T)(object)AsDecimal();
        if (typeof(T) == typeof(DateTime))
            return (T)(object)AsDateTime();
        if (typeof(T) == typeof(object))
            return (T)(object)AsJson<object>()!;
            
        throw new InvalidOperationException($"Cannot convert setting value to type {typeof(T).Name}");
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RawValue;
        yield return Type;
    }
    
    public override string ToString() => RawValue;
    
    // Factory methods for common types
    public static SettingValue FromString(string value) => new(value, SettingType.String);
    public static SettingValue FromInteger(int value) => new(value.ToString(), SettingType.Integer);
    public static SettingValue FromBoolean(bool value) => new(value.ToString(), SettingType.Boolean);
    public static SettingValue FromDecimal(decimal value) => new(value.ToString(), SettingType.Decimal);
    public static SettingValue FromDateTime(DateTime value) => new(value.ToString("O"), SettingType.DateTime);
    public static SettingValue FromJson<T>(T value) => new(JsonSerializer.Serialize(value), SettingType.Json);
}
