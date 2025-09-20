using System.Text.RegularExpressions;
using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Membership.Domain.ValueObjects;

/// <summary>
/// Value object representing an email address
/// </summary>
public class Email : ValueObject
{
    private readonly string _value;

    public string Value => _value;

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        var normalizedValue = value.Trim().ToLowerInvariant();
        
        if (!IsValid(normalizedValue))
            throw new ArgumentException("Invalid email format", nameof(value));

        _value = normalizedValue;
    }

    public override string ToString() => _value;

    public static implicit operator string(Email email) => email._value;

    private static bool IsValid(string email)
    {
        const string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}