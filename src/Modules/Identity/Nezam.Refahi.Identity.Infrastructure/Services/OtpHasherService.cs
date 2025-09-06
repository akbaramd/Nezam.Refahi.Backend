using System.Security.Cryptography;
using System.Text;

namespace Nezam.Refahi.Shared.Application.Common.Services;


public class OtpHasherService : IOtpHasherService
{
    private readonly string _secretKey;

    public OtpHasherService()
    {
     
        _secretKey = "asdasdasdasdasdasdasdasdasdasdasdasdasdasdasdewd-0asud97y30289hro23iknri-uqgw0d76a9sdtg";
    }

    /// <summary>
    /// Generates a hash for an OTP code
    /// </summary>
    /// <param name="challengeId">Challenge identifier</param>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="otpCode">OTP code to hash</param>
    /// <param name="nonce">Nonce for additional security</param>
    /// <returns>Hashed OTP code</returns>
    public Task<string> HashAsync(string challengeId, string phoneNumber, string otpCode, string nonce)
    {
        if (string.IsNullOrEmpty(challengeId))
            throw new ArgumentException("Challenge ID cannot be empty", nameof(challengeId));
            
        if (string.IsNullOrEmpty(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
            
        if (string.IsNullOrEmpty(otpCode))
            throw new ArgumentException("OTP code cannot be empty", nameof(otpCode));
            
        if (string.IsNullOrEmpty(nonce))
            throw new ArgumentException("Nonce cannot be empty", nameof(nonce));

        var input = $"{challengeId}|{phoneNumber}|{otpCode}|{nonce}";
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var secretBytes = Encoding.UTF8.GetBytes(_secretKey);

        using var hmac = new HMACSHA256(secretBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);
        var hash = Convert.ToBase64String(hashBytes);
        
        return Task.FromResult(hash);
    }

    /// <summary>
    /// Verifies an OTP code against its hash
    /// </summary>
    /// <param name="challengeId">Challenge identifier</param>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="otpCode">OTP code to verify</param>
    /// <param name="nonce">Nonce used in hash generation</param>
    /// <param name="expectedHash">Expected hash value</param>
    /// <returns>True if verification succeeds, false otherwise</returns>
    public async Task<bool> VerifyAsync(string challengeId, string phoneNumber, string otpCode, string nonce, string expectedHash)
    {
        if (string.IsNullOrEmpty(expectedHash))
            return false;

        var computedHash = await HashAsync(challengeId, phoneNumber, otpCode, nonce);
        return string.Equals(computedHash, expectedHash, StringComparison.Ordinal);
    }

    /// <summary>
    /// Generates a secure nonce
    /// </summary>
    /// <param name="length">Length of the nonce (default: 32)</param>
    /// <returns>Generated nonce</returns>
    public Task<string> GenerateNonceAsync(int length = 32)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be positive", nameof(length));

        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        
        var nonce = Convert.ToBase64String(bytes);
        return Task.FromResult(nonce);
    }
}