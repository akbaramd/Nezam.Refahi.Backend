using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Identity.Domain.ValueObjects;

/// <summary>
/// Value object representing a client identifier
/// </summary>
public class ClientId : ValueObject
{
    private readonly string _value;
    private const int MaxLength = 50;

    public string Value => _value;

    public ClientId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Client ID cannot be empty", nameof(value));

        if (value.Length > MaxLength)
            throw new ArgumentException($"Client ID cannot exceed {MaxLength} characters", nameof(value));

        if (!IsValid(value))
            throw new ArgumentException("Invalid Client ID format", nameof(value));

        _value = value;
    }

    public override string ToString() => _value;

    public static implicit operator string(ClientId clientId) => clientId._value;

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (value.Length > MaxLength) return false;
        
        // Allow alphanumeric, hyphens, and underscores
        return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9_-]+$");
    }

    protected override System.Collections.Generic.IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }


}
