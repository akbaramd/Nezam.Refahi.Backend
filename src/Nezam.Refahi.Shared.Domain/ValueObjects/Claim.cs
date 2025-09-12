using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Shared.Domain.ValueObjects;

/// <summary>
/// Value object representing a claim with type and value
/// </summary>
public class Claim : ValueObject
{
    public string Type { get; }
    public string Value { get; }
    public string? ValueType { get; }

    // Private constructor for EF Core
    private Claim()
    {
        Type = string.Empty;
        Value = string.Empty;
        ValueType = null;
    }

    public Claim(string type, string value, string? valueType = null)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Claim type cannot be empty", nameof(type));

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Claim value cannot be empty", nameof(value));

        if (type.Length > 100)
            throw new ArgumentException("Claim type cannot exceed 100 characters", nameof(type));

        if (value.Length > 1000)
            throw new ArgumentException("Claim value cannot exceed 1000 characters", nameof(value));

        Type = type.Trim();
        Value = value.Trim();
        ValueType = valueType?.Trim();
    }

    public static Claim FromString(string type, string value, string? valueType = null)
    {
        return new Claim(type, value, valueType);
    }

    public static Claim CreatePermission(string permission)
    {
        return new Claim("permission", permission, "string");
    }

    public static Claim CreateRole(string role)
    {
        return new Claim("role", role, "string");
    }

    public static Claim CreateScope(string scope)
    {
        return new Claim("scope", scope, "string");
    }

    public static Claim CreateCustom(string type, string value)
    {
        return new Claim(type, value, "string");
    }

    public bool IsPermission => Type.Equals("permission", StringComparison.OrdinalIgnoreCase);
    public bool IsRole => Type.Equals("role", StringComparison.OrdinalIgnoreCase);
    public bool IsScope => Type.Equals("scope", StringComparison.OrdinalIgnoreCase);

    public override string ToString()
    {
        return $"{Type}: {Value}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return Value;
        yield return ValueType ?? string.Empty;
    }
}
