using System.Text.RegularExpressions;
using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Shared.Domain.ValueObjects;

/// <summary>
/// Value object representing an Iranian National ID
/// </summary>
public class NationalId : ValueObject
{
    private readonly string _value;

    // Public property to access the value
    public string Value => _value;

    public NationalId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("کد ملی ارسال شده نمیتواند خالی باشد", nameof(value));

        if (!IsValid(value))
            throw new ArgumentException("فرمت کد ملی وارد شده صحیح نمیباشد", nameof(value));

        _value = value;
    }

    public override string ToString() => _value;

    public static implicit operator string(NationalId nationalId) => nationalId._value;

    public static bool IsValid(string value)
    {
        // Basic validation: must be 10 digits
        if (!Regex.IsMatch(value, @"^\d{10}$"))
            return false;

        return true;
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

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return _value;
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
