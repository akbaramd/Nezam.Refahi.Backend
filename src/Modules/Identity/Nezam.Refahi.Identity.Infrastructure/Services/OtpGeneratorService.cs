using System.Security.Cryptography;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

public class OtpGeneratorService : IOtpGeneratorService
{
    /// <summary>
    /// Generates a secure 6-digit OTP code
    /// </summary>
    /// <returns>Generated OTP code</returns>
    public Task<string> GenerateOtpAsync()
    {
        return Task.FromResult(GenerateSecureOtp(6));
    }

    /// <summary>
    /// Generates a secure OTP code with specified length
    /// </summary>
    /// <param name="length">Length of the OTP code</param>
    /// <returns>Generated OTP code</returns>
    public Task<string> GenerateOtpAsync(int length)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be positive", nameof(length));

        return Task.FromResult(GenerateSecureOtp(length));
    }

    /// <summary>
    /// Generates a secure OTP code using cryptographically secure random number generator
    /// </summary>
    /// <param name="length">Length of the OTP code</param>
    /// <returns>Generated OTP code</returns>
    private static string GenerateSecureOtp(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);

        var otp = "";
        for (int i = 0; i < length; i++)
        {
            otp += (bytes[i] % 10).ToString();
        }

        return otp;
    }
}