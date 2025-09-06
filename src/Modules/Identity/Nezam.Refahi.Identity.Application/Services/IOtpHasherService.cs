namespace Nezam.Refahi.Shared.Application.Common.Services;

/// <summary>
/// Interface for OTP hashing service
/// </summary>
public interface IOtpHasherService
{
  /// <summary>
  /// Generates a hash for an OTP code
  /// </summary>
  /// <param name="challengeId">Challenge identifier</param>
  /// <param name="phoneNumber">Phone number</param>
  /// <param name="otpCode">OTP code to hash</param>
  /// <param name="nonce">Nonce for additional security</param>
  /// <returns>Hashed OTP code</returns>
  Task<string> HashAsync(string challengeId, string phoneNumber, string otpCode, string nonce);
    
  /// <summary>
  /// Verifies an OTP code against its hash
  /// </summary>
  /// <param name="challengeId">Challenge identifier</param>
  /// <param name="phoneNumber">Phone number</param>
  /// <param name="otpCode">OTP code to verify</param>
  /// <param name="nonce">Nonce used in hash generation</param>
  /// <param name="expectedHash">Expected hash value</param>
  /// <returns>True if verification succeeds, false otherwise</returns>
  Task<bool> VerifyAsync(string challengeId, string phoneNumber, string otpCode, string nonce, string expectedHash);
    
  /// <summary>
  /// Generates a secure nonce
  /// </summary>
  /// <param name="length">Length of the nonce (default: 32)</param>
  /// <returns>Generated nonce</returns>
  Task<string> GenerateNonceAsync(int length = 32);
}
