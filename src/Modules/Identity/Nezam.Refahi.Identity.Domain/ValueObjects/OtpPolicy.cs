using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Identity.Domain.ValueObjects;

/// <summary>
/// Value object representing OTP policy configuration
/// </summary>
public class OtpPolicy : ValueObject
{
    public int Length { get; }
    public int TtlSeconds { get; }
    public int MaxVerifyAttempts { get; }
    public int MaxResends { get; }
    public int MaxPerPhonePerHour { get; }
    public int MaxPerIpPerHour { get; }

    public OtpPolicy(
        int length = 6,
        int ttlSeconds = 180,
        int maxVerifyAttempts = 3,
        int maxResends = 3,
        int maxPerPhonePerHour = 5,
        int maxPerIpPerHour = 10)
    {
        if (length <= 0)
            throw new ArgumentException("OTP length must be positive", nameof(length));
            
        if (ttlSeconds <= 0)
            throw new ArgumentException("TTL must be positive", nameof(ttlSeconds));
            
        if (maxVerifyAttempts <= 0)
            throw new ArgumentException("Max verify attempts must be positive", nameof(maxVerifyAttempts));
            
        if (maxResends <= 0)
            throw new ArgumentException("Max resends must be positive", nameof(maxResends));
            
        if (maxPerPhonePerHour <= 0)
            throw new ArgumentException("Max per phone per hour must be positive", nameof(maxPerPhonePerHour));
            
        if (maxPerIpPerHour <= 0)
            throw new ArgumentException("Max per IP per hour must be positive", nameof(maxPerIpPerHour));

        Length = length;
        TtlSeconds = ttlSeconds;
        MaxVerifyAttempts = maxVerifyAttempts;
        MaxResends = maxResends;
        MaxPerPhonePerHour = maxPerPhonePerHour;
        MaxPerIpPerHour = maxPerIpPerHour;
    }

    /// <summary>
    /// Creates a default OTP policy
    /// </summary>
    public static OtpPolicy Default => new OtpPolicy();

    /// <summary>
    /// Creates a strict OTP policy for high-security scenarios
    /// </summary>
    public static OtpPolicy Strict => new OtpPolicy(
        length: 8,
        ttlSeconds: 120,
        maxVerifyAttempts: 2,
        maxResends: 1,
        maxPerPhonePerHour: 3,
        maxPerIpPerHour: 5
    );

    /// <summary>
    /// Creates a relaxed OTP policy for user-friendly scenarios
    /// </summary>
    public static OtpPolicy Relaxed => new OtpPolicy(
        length: 6,
        ttlSeconds: 300,
        maxVerifyAttempts: 5,
        maxResends: 5,
        maxPerPhonePerHour: 10,
        maxPerIpPerHour: 20
    );

    protected override System.Collections.Generic.IEnumerable<object> GetEqualityComponents()
    {
        yield return Length;
        yield return TtlSeconds;
        yield return MaxVerifyAttempts;
        yield return MaxResends;
        yield return MaxPerPhonePerHour;
        yield return MaxPerIpPerHour;
    }
}
