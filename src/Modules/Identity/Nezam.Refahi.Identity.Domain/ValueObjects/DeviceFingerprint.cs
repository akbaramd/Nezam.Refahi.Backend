using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Identity.Domain.ValueObjects;

/// <summary>
/// Value object representing a device fingerprint for security purposes
/// </summary>
public class DeviceFingerprint : ValueObject
{
    private readonly string _value;

    // Public property to access the value
    public string Value => _value;

    public DeviceFingerprint(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Device fingerprint cannot be empty", nameof(value));

        if (value.Length < 10)
            throw new ArgumentException("Device fingerprint must be at least 10 characters", nameof(value));

        if (value.Length > 500)
            throw new ArgumentException("Device fingerprint cannot exceed 500 characters", nameof(value));

        _value = value;
    }

    public override string ToString() => _value;

    public static implicit operator string(DeviceFingerprint deviceFingerprint) => deviceFingerprint._value;

    public bool Equals(DeviceFingerprint? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((DeviceFingerprint)obj);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

    public static bool operator ==(DeviceFingerprint? left, DeviceFingerprint? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(DeviceFingerprint? left, DeviceFingerprint? right)
    {
        return !(left == right);
    }
}