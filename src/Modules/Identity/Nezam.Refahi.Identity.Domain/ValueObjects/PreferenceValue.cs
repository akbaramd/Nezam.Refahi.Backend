using System.Text.Json;
using MCA.SharedKernel.Domain;
using Nezam.Refahi.Identity.Domain.Enums;

namespace Nezam.Refahi.Identity.Domain.ValueObjects;

/// <summary>
/// Represents a preference value with type validation and conversion capabilities
/// </summary>
public class PreferenceValue : ValueObject
{
    public string RawValue { get; private set; } = null!;
    public PreferenceType Type { get; private set; }
    
    // Private parameterless constructor for EF Core
    private PreferenceValue() { }
    
    public PreferenceValue(string value, PreferenceType type)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
            
        if (string.IsNullOrWhiteSpace(value) && type != PreferenceType.String)
            throw new ArgumentException("Value cannot be empty for non-string types", nameof(value));
            
        RawValue = value;
        Type = type;
        
        // Validate the value based on type
        ValidateValue(value, type);
    }
    
    private static void ValidateValue(string value, PreferenceType type)
    {
        switch (type)
        {
            case PreferenceType.Integer:
                if (!int.TryParse(value, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid integer", nameof(value));
                break;
                
            case PreferenceType.Boolean:
                if (!bool.TryParse(value, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid boolean", nameof(value));
                break;
                
            case PreferenceType.Decimal:
                if (!decimal.TryParse(value, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid decimal", nameof(value));
                break;
                
            case PreferenceType.DateTime:
                if (!DateTime.TryParse(value, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid datetime", nameof(value));
                break;
                
            case PreferenceType.Json:
                try
                {
                    JsonDocument.Parse(value);
                }
                catch (JsonException)
                {
                    throw new ArgumentException($"Value '{value}' is not valid JSON", nameof(value));
                }
                break;
                
            case PreferenceType.Email:
                if (!IsValidEmail(value))
                    throw new ArgumentException($"Value '{value}' is not a valid email address", nameof(value));
                break;
                
            case PreferenceType.Url:
                if (!Uri.TryCreate(value, UriKind.Absolute, out _))
                    throw new ArgumentException($"Value '{value}' is not a valid URL", nameof(value));
                break;
                
            case PreferenceType.PhoneNumber:
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
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RawValue;
        yield return Type;
    }
    
    public override string ToString() => RawValue;
    
    // Factory methods for common types
    public static PreferenceValue FromString(string value) => new(value, PreferenceType.String);
    public static PreferenceValue FromInteger(int value) => new(value.ToString(), PreferenceType.Integer);
    public static PreferenceValue FromBoolean(bool value) => new(value.ToString(), PreferenceType.Boolean);
    public static PreferenceValue FromDecimal(decimal value) => new(value.ToString(), PreferenceType.Decimal);
    public static PreferenceValue FromDateTime(DateTime value) => new(value.ToString("O"), PreferenceType.DateTime);
    public static PreferenceValue FromJson<T>(T value) => new(JsonSerializer.Serialize(value), PreferenceType.Json);
}
