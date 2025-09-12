namespace Nezam.Refahi.Identity.Application.Services.Contracts;

/// <summary>
/// Service for generating and validating one-time passwords (OTPs)
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Generates a new OTP for a user
    /// </summary>
    /// <param name="userId">The user's identifier (could be phone number, email, etc.)</param>
    /// <param name="purpose">The purpose of the OTP (e.g., "login", "reset-password")</param>
    /// <param name="expiryMinutes">How long the OTP should be valid for in minutes</param>
    /// <returns>The generated OTP</returns>
    Task<string> GenerateOtpAsync(string userId, string purpose, int expiryMinutes = 5);
    
    /// <summary>
    /// Validates an OTP for a user
    /// </summary>
    /// <param name="userId">The user's identifier</param>
    /// <param name="otp">The OTP to validate</param>
    /// <param name="purpose">The purpose of the OTP</param>
    /// <returns>True if the OTP is valid, false otherwise</returns>
    Task<bool> ValidateOtpAsync(string userId, string otp, string purpose);
}
