using System.Text.RegularExpressions;
using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Shared.Domain.ValueObjects;

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
        
        // Iranian mobile numbers: 09XXXXXXXXX (11 digits) - preferred format
        if (digitsOnly.StartsWith("09") && digitsOnly.Length == 11)
            return true;
            
        // International format: +989XXXXXXXXX
        if (value.StartsWith("+98") && digitsOnly.Length == 11)
            return true;
            
        // Also accept the normalized international format
        if (digitsOnly.StartsWith("989") && digitsOnly.Length == 12)
            return true;
            
        // Accept 10-digit numbers (will be normalized to 09XXXXXXXXX)
        if (digitsOnly.Length == 10 && !digitsOnly.StartsWith("0"))
            return true;
            
        return false;
    }

    private static string Normalize(string value)
    {
        // Convert to simple Iranian mobile format (09XXXXXXXXX)
        var digitsOnly = Regex.Replace(value, @"[^\d]", "");
        
        // Handle Iranian mobile numbers starting with 09
        if (digitsOnly.StartsWith("09") && digitsOnly.Length == 11)
        {
            return digitsOnly; // Already in correct format
        }
        
        // Handle international format starting with +98
        if (value.StartsWith("+98"))
        {
            return "0" + digitsOnly.Substring(2); // Convert +98XXXXXXXXX to 09XXXXXXXXX
        }
        
        // Handle numbers starting with 989 (without +)
        if (digitsOnly.StartsWith("989") && digitsOnly.Length == 12)
        {
            return "0" + digitsOnly.Substring(2); // Convert 989XXXXXXXXX to 09XXXXXXXXX
        }
        
        // Handle numbers starting with 98 (without +)
        if (digitsOnly.StartsWith("98") && digitsOnly.Length == 11)
        {
            return "0" + digitsOnly.Substring(1); // Convert 98XXXXXXXXX to 09XXXXXXXXX
        }
        
        // Default: assume it's an Iranian number and add 0
        if (digitsOnly.Length == 10)
        {
            return "0" + digitsOnly; // Convert XXXXXXXXX to 09XXXXXXXXX
        }
        
        // If already 11 digits, return as is
        return digitsOnly;
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