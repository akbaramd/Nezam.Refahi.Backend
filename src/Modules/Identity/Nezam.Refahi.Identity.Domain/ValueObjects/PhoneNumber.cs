using System.Text.RegularExpressions;
using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Identity.Domain.ValueObjects;

/// <summary>
/// Value object representing a phone number
/// </summary>
public class PhoneNumber : ValueObject
{
    private readonly string _value;

    // Public property to access the value
    public string Value => _value;

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));

        // First normalize the input, then validate the normalized form
        var normalizedValue = Normalize(value);
        
        if (!IsValid(normalizedValue))
            throw new ArgumentException("Invalid phone number format", nameof(value));

        _value = normalizedValue;
    }

    public override string ToString() => _value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber._value;

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        // Remove all non-digit characters for validation
        var digitsOnly = Regex.Replace(value, @"[^\d]", "");
        
        // Iranian mobile numbers: 09XXXXXXXXX (11 digits)
        if (digitsOnly.StartsWith("09") && digitsOnly.Length == 11)
            return true;
            
        // International format: +989XXXXXXXXX
        if (value.StartsWith("+98") && digitsOnly.Length == 11)
            return true;
            
        // Also accept the normalized international format
        if (digitsOnly.StartsWith("989") && digitsOnly.Length == 12)
            return true;
            
        return false;
    }

    private static string Normalize(string value)
    {
        // Convert to international format
        var digitsOnly = Regex.Replace(value, @"[^\d]", "");
        
        // Handle Iranian mobile numbers starting with 09
        if (digitsOnly.StartsWith("09"))
        {
            return "+98" + digitsOnly.Substring(1);
        }
        
        // Handle international format starting with +98
        if (value.StartsWith("+98"))
        {
            return value;
        }
        
        // Handle numbers starting with 989 (without +)
        if (digitsOnly.StartsWith("989"))
        {
            return "+" + digitsOnly;
        }
        
        // Handle numbers starting with 98 (without +)
        if (digitsOnly.StartsWith("98") && digitsOnly.Length == 11)
        {
            return "+" + digitsOnly;
        }
        
        // Default: assume it's an Iranian number and add +98
        return "+98" + digitsOnly;
    }

    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((PhoneNumber)obj);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public static bool operator ==(PhoneNumber? left, PhoneNumber? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(PhoneNumber? left, PhoneNumber? right)
    {
        return !(left == right);
    }
}