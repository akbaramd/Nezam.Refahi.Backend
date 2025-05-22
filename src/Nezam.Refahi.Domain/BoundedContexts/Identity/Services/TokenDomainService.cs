using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Services;

/// <summary>
/// Domain service for token-related operations that don't naturally belong to the UserToken entity
/// </summary>
public class TokenDomainService
{
    private readonly IUserTokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;

    public TokenDomainService(IUserTokenRepository tokenRepository, IUserRepository userRepository)
    {
        _tokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Generates a new OTP code for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="expiresInMinutes">Minutes until OTP expiration (default: 5)</param>
    /// <param name="deviceId">Optional device identifier</param>
    /// <param name="ipAddress">Optional IP address</param>
    /// <returns>The generated OTP code</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user doesn't exist</exception>
    public async Task<string> GenerateOtpAsync(Guid userId, int expiresInMinutes = 5, string? deviceId = null, string? ipAddress = null)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        // Revoke any existing OTP tokens for this user
        await _tokenRepository.RevokeAllUserTokensOfTypeAsync(userId, "OTP");

        // Generate a random 6-digit OTP code
        string otpCode = GenerateRandomOtp();

        // Create and store the token
        var token = new UserToken(userId, otpCode, "OTP", expiresInMinutes, deviceId, ipAddress);
        await _tokenRepository.AddAsync(token);

        return otpCode;
    }

    /// <summary>
    /// Validates an OTP code for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="otpCode">The OTP code to validate</param>
    /// <returns>True if the OTP is valid, false otherwise</returns>
    public async Task<bool> ValidateOtpAsync(Guid userId, string otpCode)
    {
        if (string.IsNullOrWhiteSpace(otpCode))
            return false;

        var token = await _tokenRepository.GetByTokenValueAsync(otpCode, "OTP");
        if (token == null || token.UserId != userId)
            return false;

        if (!token.IsValid())
            return false;

        // Mark the token as used
        token.MarkAsUsed();
        await _tokenRepository.UpdateAsync(token);

        return true;
    }

    /// <summary>
    /// Generates a refresh token for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="expiresInMinutes">Minutes until token expiration (default: 10080 = 7 days)</param>
    /// <param name="deviceId">Optional device identifier</param>
    /// <param name="ipAddress">Optional IP address</param>
    /// <returns>The generated refresh token</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user doesn't exist</exception>
    public async Task<string> GenerateRefreshTokenAsync(Guid userId, int expiresInMinutes = 10080, string? deviceId = null, string? ipAddress = null)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        // Generate a random refresh token
        string refreshToken = GenerateRandomToken();

        // Create and store the token
        var token = new UserToken(userId, refreshToken, "RefreshToken", expiresInMinutes, deviceId, ipAddress);
        await _tokenRepository.AddAsync(token);

        return refreshToken;
    }

    /// <summary>
    /// Validates a refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to validate</param>
    /// <returns>The user ID if the token is valid, null otherwise</returns>
    public async Task<Guid?> ValidateRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var token = await _tokenRepository.GetByTokenValueAsync(refreshToken, "RefreshToken");
        if (token == null || !token.IsValid())
            return null;

        return token.UserId;
    }

    /// <summary>
    /// Revokes a specific refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to revoke</param>
    /// <returns>True if the token was found and revoked, false otherwise</returns>
    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var token = await _tokenRepository.GetByTokenValueAsync(refreshToken, "RefreshToken");
        if (token == null)
            return false;

        token.Revoke();
        await _tokenRepository.UpdateAsync(token);

        return true;
    }

    /// <summary>
    /// Revokes all refresh tokens for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <returns>Number of tokens revoked</returns>
    public async Task<int> RevokeAllRefreshTokensAsync(Guid userId)
    {
        return await _tokenRepository.RevokeAllUserTokensOfTypeAsync(userId, "RefreshToken");
    }

    /// <summary>
    /// Stores a JWT token reference for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="jwtId">The JWT ID (jti claim)</param>
    /// <param name="expiresInMinutes">Minutes until token expiration</param>
    /// <param name="deviceId">Optional device identifier</param>
    /// <param name="ipAddress">Optional IP address</param>
    /// <returns>The created token entity</returns>
    public async Task<UserToken> StoreJwtReferenceAsync(Guid userId, string jwtId, int expiresInMinutes, string? deviceId = null, string? ipAddress = null)
    {
        var token = new UserToken(userId, jwtId, "JWT", expiresInMinutes, deviceId, ipAddress);
        await _tokenRepository.AddAsync(token);
        return token;
    }

    /// <summary>
    /// Validates if a JWT ID is valid (not revoked)
    /// </summary>
    /// <param name="jwtId">The JWT ID to validate</param>
    /// <returns>True if the JWT is valid, false otherwise</returns>
    public async Task<bool> ValidateJwtAsync(string jwtId)
    {
        if (string.IsNullOrWhiteSpace(jwtId))
            return false;

        var token = await _tokenRepository.GetByTokenValueAsync(jwtId, "JWT");
        if (token == null)
            return false;

        return !token.IsRevoked && !token.IsExpired();
    }

    /// <summary>
    /// Revokes a JWT token
    /// </summary>
    /// <param name="jwtId">The JWT ID to revoke</param>
    /// <returns>True if the token was found and revoked, false otherwise</returns>
    public async Task<bool> RevokeJwtAsync(string jwtId)
    {
        if (string.IsNullOrWhiteSpace(jwtId))
            return false;

        var token = await _tokenRepository.GetByTokenValueAsync(jwtId, "JWT");
        if (token == null)
            return false;

        token.Revoke();
        await _tokenRepository.UpdateAsync(token);

        return true;
    }

    /// <summary>
    /// Cleans up expired tokens
    /// </summary>
    /// <returns>Number of tokens removed</returns>
    public async Task<int> CleanupExpiredTokensAsync()
    {
        return await _tokenRepository.CleanupExpiredTokensAsync();
    }

    #region Helper Methods

    /// <summary>
    /// Generates a random 6-digit OTP code
    /// </summary>
    private string GenerateRandomOtp()
    {
        // Generate a random 6-digit number
        using var rng = RandomNumberGenerator.Create();
        var randomNumber = new byte[4];
        rng.GetBytes(randomNumber);
        var value = BitConverter.ToUInt32(randomNumber, 0);
        return (value % 1000000).ToString("D6"); // Ensures 6 digits with leading zeros
    }

    /// <summary>
    /// Generates a random token string
    /// </summary>
    private string GenerateRandomToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[32]; // 256 bits
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    #endregion
}
