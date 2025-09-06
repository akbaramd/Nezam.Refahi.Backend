using System.Net;
using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Identity.Domain.ValueObjects;

/// <summary>
/// Value object representing an IP address
/// </summary>
public class IpAddress : ValueObject
{
    private readonly string _value;

    public string Value => _value;

    public IpAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("IP address cannot be empty", nameof(value));

        if (!IsValid(value))
            throw new ArgumentException("Invalid IP address format", nameof(value));

        _value = value;
    }

    public override string ToString() => _value;

    public static implicit operator string(IpAddress ipAddress) => ipAddress._value;

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        // Check if it's a valid IPv4 or IPv6 address
        return IPAddress.TryParse(value, out _);
    }

    /// <summary>
    /// Checks if the IP address is a private/local address
    /// </summary>
    public bool IsPrivate
    {
        get
        {
            if (System.Net.IPAddress.TryParse(_value, out var ip))
            {
                var bytes = ip.GetAddressBytes();
                
                // IPv4 private ranges
                if (bytes.Length == 4)
                {
                    return (bytes[0] == 10) ||
                           (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                           (bytes[0] == 192 && bytes[1] == 168);
                }
                
                // IPv6 private ranges (simplified check)
                if (bytes.Length == 16)
                {
                    return bytes[0] == 0xfd || bytes[0] == 0xfe;
                }
            }
            
            return false;
        }
    }

    /// <summary>
    /// Checks if the IP address is a loopback address
    /// </summary>
    public bool IsLoopback
    {
        get
        {
            if (System.Net.IPAddress.TryParse(_value, out var ip))
            {
                return System.Net.IPAddress.IsLoopback(ip);
            }
            return false;
        }
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    protected override System.Collections.Generic.IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }


}
