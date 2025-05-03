using System;
using System.Text.RegularExpressions;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;

/// <summary>
/// Value object representing an Iranian National ID
/// </summary>
public class NationalId : IEquatable<NationalId>
{
    private readonly string _value;

    // Public property to access the value
    public string Value => _value;

    public NationalId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("National ID cannot be empty", nameof(value));

        if (!IsValid(value))
            throw new ArgumentException("Invalid National ID format", nameof(value));

        _value = value;
    }

    public override string ToString() => _value;

    public static implicit operator string(NationalId nationalId) => nationalId._value;

    public static bool IsValid(string value)
    {
        // Basic validation: must be 10 digits
        if (!Regex.IsMatch(value, @"^\d{10}$"))
            return false;

        // Advanced validation for Iranian National ID
        // Algorithm: https://en.wikipedia.org/wiki/Iranian_National_Number
        
        // Check for invalid repeated digits
        bool allDigitsSame = true;
        for (int i = 1; i < value.Length; i++)
        {
            if (value[i] != value[0])
            {
                allDigitsSame = false;
                break;
            }
        }
        if (allDigitsSame)
            return false;
            
        // Check digit validation
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (value[i] - '0') * (10 - i);
        }
        
        int remainder = sum % 11;
        int checkDigit = value[9] - '0';
        
        if (remainder < 2)
            return checkDigit == remainder;
        else
            return checkDigit == 11 - remainder;
    }

    public bool Equals(NationalId? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((NationalId)obj);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool operator ==(NationalId? left, NationalId? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(NationalId? left, NationalId? right)
    {
        return !(left == right);
    }
}
