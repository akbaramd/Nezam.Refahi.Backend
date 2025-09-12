namespace Nezam.Refahi.Identity.Application.Services.Contracts;

/// <summary>
/// Interface for OTP generation service
/// </summary>
public interface IOtpGeneratorService
{
  /// <summary>
  /// Generates a secure 6-digit OTP code
  /// </summary>
  /// <returns>Generated OTP code</returns>
  Task<string> GenerateOtpAsync();

  /// <summary>
  /// Generates a secure OTP code with specified length
  /// </summary>
  /// <param name="length">Length of the OTP code</param>
  /// <returns>Generated OTP code</returns>
  Task<string> GenerateOtpAsync(int length);
}
