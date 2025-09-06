namespace Nezam.Refahi.Identity.Domain.Policies;

/// <summary>
/// Policy for OTP challenge operations
/// </summary>
public class OtpChallengePolicy
{
    public int MaxPerPhonePerHour { get; }
    public int MaxPerIpPerHour { get; }
    public int MaxPerDevicePerHour { get; }
    public int VerifyAttempts { get; }
    public int ResendLimit { get; }
    public int TtlSeconds { get; }
    public int MinResendIntervalSeconds { get; }

    public OtpChallengePolicy(
        int maxPerPhonePerHour = 5,
        int maxPerIpPerHour = 10,
        int maxPerDevicePerHour = 3,
        int verifyAttempts = 3,
        int resendLimit = 3,
        int ttlSeconds = 180,
        int minResendIntervalSeconds = 60)
    {
        if (maxPerPhonePerHour <= 0)
            throw new ArgumentException("Max per phone per hour must be positive", nameof(maxPerPhonePerHour));
            
        if (maxPerIpPerHour <= 0)
            throw new ArgumentException("Max per IP per hour must be positive", nameof(maxPerIpPerHour));
            
        if (maxPerDevicePerHour <= 0)
            throw new ArgumentException("Max per device per hour must be positive", nameof(maxPerDevicePerHour));
            
        if (verifyAttempts <= 0)
            throw new ArgumentException("Verify attempts must be positive", nameof(verifyAttempts));
            
        if (resendLimit <= 0)
            throw new ArgumentException("Resend limit must be positive", nameof(resendLimit));
            
        if (ttlSeconds <= 0)
            throw new ArgumentException("TTL seconds must be positive", nameof(ttlSeconds));
            
        if (minResendIntervalSeconds <= 0)
            throw new ArgumentException("Min resend interval must be positive", nameof(minResendIntervalSeconds));

        MaxPerPhonePerHour = maxPerPhonePerHour;
        MaxPerIpPerHour = maxPerIpPerHour;
        MaxPerDevicePerHour = maxPerDevicePerHour;
        VerifyAttempts = verifyAttempts;
        ResendLimit = resendLimit;
        TtlSeconds = ttlSeconds;
        MinResendIntervalSeconds = minResendIntervalSeconds;
    }

    /// <summary>
    /// Creates a default OTP challenge policy
    /// </summary>
    public static OtpChallengePolicy Default => new OtpChallengePolicy();

    /// <summary>
    /// Creates a strict OTP challenge policy for high-security scenarios
    /// </summary>
    public static OtpChallengePolicy Strict => new OtpChallengePolicy(
        maxPerPhonePerHour: 3,
        maxPerIpPerHour: 5,
        maxPerDevicePerHour: 2,
        verifyAttempts: 2,
        resendLimit: 1,
        ttlSeconds: 120,
        minResendIntervalSeconds: 120
    );

    /// <summary>
    /// Creates a relaxed OTP challenge policy for user-friendly scenarios
    /// </summary>
    public static OtpChallengePolicy Relaxed => new OtpChallengePolicy(
        maxPerPhonePerHour: 10,
        maxPerIpPerHour: 20,
        maxPerDevicePerHour: 5,
        verifyAttempts: 5,
        resendLimit: 5,
        ttlSeconds: 300,
        minResendIntervalSeconds: 30
    );
}
