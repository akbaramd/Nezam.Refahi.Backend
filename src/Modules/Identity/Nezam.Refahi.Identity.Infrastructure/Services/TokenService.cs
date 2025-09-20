// -----------------------------------------------------------------------------
// TokenService.cs - Production-grade DDD + EF Core token system
// -----------------------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Production-grade token service implementing DDD + EF Core best practices
/// Supports JWT access tokens (5-15 min TTL) and refresh token rotation
/// </summary>
public class TokenService : ITokenService
{
    // ========================================================================
    // Dependencies
    // ========================================================================
    
    private readonly IConfiguration _configuration;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDistributedCache? _distributedCache;
    private readonly ILogger<TokenService> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    
    // ========================================================================
    // Configuration Constants
    // ========================================================================
    
    private const string JWT_DENY_LIST_KEY_PREFIX = "jwt_deny:";
    private const int REFRESH_TOKEN_LENGTH = 32; // 256 bits
    private const string PEPPER_CONFIG_KEY = "TokenService:Pepper";
    
    // ========================================================================
    // Constructor
    // ========================================================================
    
    public TokenService(
        IConfiguration configuration,
        IUserTokenRepository userTokenRepository,
        IUserRepository userRepository,
        IDistributedCache? distributedCache = null,
        ILogger<TokenService>? logger = null)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _userTokenRepository = userTokenRepository ?? throw new ArgumentNullException(nameof(userTokenRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _distributedCache = distributedCache;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    // ========================================================================
    // JWT Access Token Operations (Stateless, 5-15 min TTL)
    // ========================================================================
    
    public async Task<(string AccessToken, int ExpiryMinutes, string JwtId)> GenerateAccessTokenAsync(
        User user, 
        string? deviceFingerprint = null, 
        string? ipAddress = null, 
        string? userAgent = null, 
        int expiryMinutes = 15)
    {
        if (expiryMinutes <= 0 || expiryMinutes > 60)
            throw new ArgumentException("Access token expiry must be between 1-60 minutes", nameof(expiryMinutes));
            
        // Get signing key and algorithm
        var (signingKey, algorithm) = GetSigningCredentials();
        var now = DateTime.UtcNow;
        var exp = now.AddMinutes(expiryMinutes);
        
        // Generate unique JWT ID
        var jwtId = Guid.NewGuid().ToString("N");
        
        // Build claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, jwtId),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Exp, new DateTimeOffset(exp).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("user_id", user.Id.ToString()),
            new("national_id", user.NationalId?.Value ?? ""),
        };
        
        // Add phone number if available
        if (!string.IsNullOrWhiteSpace(user.PhoneNumber?.Value))
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber.Value));
        
        // Add roles
        AddRoleClaims(user, claims);
        
        // Create token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = exp,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(signingKey, algorithm)
        };
        
        // Generate and return token
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = _tokenHandler.WriteToken(token);
        
        _logger.LogDebug("Generated access token for user {UserId} with jti {JwtId}, expires in {ExpiryMinutes} minutes", 
            user.Id, jwtId, expiryMinutes);
        
        // Create refresh token entity
        var refreshToken = UserToken.CreateAccessToken(
          user.Id, 
          tokenString, 
          expiresInMinutes:expiryMinutes, 
          deviceFingerprint: deviceFingerprint,
          ipAddress: ipAddress,
          userAgent: userAgent);
        
        // Save to database
        await _userTokenRepository.AddAsync(refreshToken,true);
      
        
        return (tokenString ,  expiryMinutes,jwtId);
    }
    
    public (bool IsValid, string? JwtId, Guid? UserId) ValidateAccessToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return (false, null, null);
        
        try
        {
            // Get signing key and algorithm
            var (signingKey, algorithm) = GetSigningCredentials();
            
            // Configure validation parameters
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };
            
            // Validate token
            var principal = _tokenHandler.ValidateToken(token, parameters, out _);
            
            // Extract claims
            var jwtId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var userIdClaim = principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? principal.FindFirstValue("user_id");
            
            if (string.IsNullOrEmpty(jwtId) || string.IsNullOrEmpty(userIdClaim))
                return (false, null, null);
                
            if (!Guid.TryParse(userIdClaim, out var parsedUserId))
                return (false, null, null);
            
            // Check if token is in deny list (for emergency revocation)
            if (IsJwtRevoked(jwtId))
            {
                _logger.LogWarning("Access token {JwtId} is in deny list", jwtId);
                return (false, null, null);
            }
            
            return (true, jwtId, parsedUserId);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Access token validation failed");
            return (false, null, null);
        }
    }
    
    // ========================================================================
    // Refresh Token Operations (Stateful, 30-90 day TTL with rotation)
    // ========================================================================
    
    public async Task<(string RawToken, string HashedToken, Guid TokenId)> GenerateRefreshTokenAsync(
        Guid userId, 
        string? deviceFingerprint = null, 
        string? ipAddress = null, 
        string? userAgent = null, 
        int expiryDays = 30)
    {
        if (expiryDays <= 0 || expiryDays > 90)
            throw new ArgumentException("Refresh token expiry must be between 1-90 days", nameof(expiryDays));
        
        // Generate cryptographically secure random token
        var rawToken = GenerateSecureToken(REFRESH_TOKEN_LENGTH);
        var pepper = _configuration[PEPPER_CONFIG_KEY] ?? "";
        
        // Create refresh token entity
        var refreshToken = UserToken.CreateRefreshToken(
            userId, 
            rawToken, 
            expiryDays, 
            deviceFingerprint: deviceFingerprint,
            ipAddress: ipAddress,
            userAgent: userAgent,
            pepper: pepper);
        
        // Save to database
        await _userTokenRepository.AddAsync(refreshToken,true);
        
        
        _logger.LogDebug("Generated refresh token for user {UserId}, expires in {ExpiryDays} days", 
            userId, expiryDays);
        
        return (rawToken, refreshToken.TokenValue, refreshToken.Id);
    }
    
    public async Task<RefreshTokenValidationResult> ValidateAndRotateRefreshTokenAsync(
        string rawToken, 
        string? deviceFingerprint = null, 
        string? ipAddress = null, 
        string? userAgent = null)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return new RefreshTokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Refresh token is required"
            };
        }
        
        try
        {
            // Find the refresh token by trying to match against all active refresh tokens
            // We need to search across all users since we don't know the user ID yet
            var refreshTokens = await _userTokenRepository.GetAllActiveTokensByTypeAsync("RefreshToken");
            UserToken? matchingToken = null;
            
            var pepper = _configuration[PEPPER_CONFIG_KEY] ?? "";
            
            foreach (var token in refreshTokens)
            {
                if (token.ValidateToken(rawToken, pepper))
                {
                    matchingToken = token;
                    break;
                }
            }
            
            if (matchingToken == null)
            {
                _logger.LogWarning("Invalid refresh token provided");
                return new RefreshTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Invalid refresh token"
                };
            }
            
            // Check if token is still valid
            if (!matchingToken.IsValid())
            {
                _logger.LogWarning("Refresh token {TokenId} is invalid (expired, used, or revoked)", matchingToken.Id);
                return new RefreshTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Refresh token is no longer valid"
                };
            }
            
            // Check binding
            if (!matchingToken.ValidateBinding(deviceFingerprint, ipAddress, userAgent))
            {
                _logger.LogWarning("Refresh token {TokenId} binding validation failed", matchingToken.Id);
                return new RefreshTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token binding validation failed"
                };
            }
            
            // Check for reuse detection
            if (matchingToken.IsUsed)
            {
                _logger.LogWarning("Refresh token {TokenId} reuse detected, revoking session family {SessionFamilyId}", 
                    matchingToken.Id, matchingToken.SessionFamilyId);
                
                // Revoke entire session family
                await _userTokenRepository.RevokeSessionFamilyAsync(matchingToken.SessionFamilyId!.Value);
                
                return new RefreshTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token reuse detected - session compromised",
                    IsReuseDetected = true,
                    IsSessionCompromised = true
                };
            }
            
            // Mark token as used
            matchingToken.MarkAsUsed();
            await _userTokenRepository.UpdateAsync(matchingToken);
            
            // Get user for new token generation
            var user = await _userRepository.FindOneAsync(x=>x.Id==matchingToken.UserId);
            if (user == null)
            {
                return new RefreshTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "UserDetail not found"
                };
            }
            
            // Generate new access token
            var (newAccessToken,newExpiryMinutes,jwtId) =  await GenerateAccessTokenAsync(user);
            
            // Generate new refresh token (rotation)
            var (newRawToken, newHashedToken, newTokenId) = await GenerateRefreshTokenAsync(
                user.Id,
                deviceFingerprint,
                ipAddress,
                userAgent,
                30); // 30 days expiry
            
            // Update the new refresh token to be in the same session family
            var newRefreshToken = await _userTokenRepository.FindOneAsync(x=>x.Id==newTokenId);
            if (newRefreshToken != null)
            {
                // This would require updating the entity, but for now we'll work with what we have
                _logger.LogDebug("Generated new refresh token {NewTokenId} in session family {SessionFamilyId}", 
                    newTokenId, matchingToken.SessionFamilyId);
            }
            
            _logger.LogInformation("Successfully rotated refresh token for user {UserId}", user.Id);
            
            return new RefreshTokenValidationResult
            {
                IsValid = true,
                UserId = user.Id,
                NewAccessToken = newAccessToken,
                NewRefreshToken = newRawToken,
                NewJwtId = jwtId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating and rotating refresh token");
            return new RefreshTokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Token validation failed"
            };
        }
    }
    
    // ========================================================================
    // Token Revocation and Cleanup
    // ========================================================================
    
    public async Task RevokeJwtAsync(string jwtId, TimeSpan remainingLifetime)
    {
        if (string.IsNullOrWhiteSpace(jwtId))
            return;
        
        // Add to distributed cache deny list
        if (_distributedCache != null)
        {
            var cacheKey = $"{JWT_DENY_LIST_KEY_PREFIX}{jwtId}";
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = remainingLifetime
            };
            
            await _distributedCache.SetStringAsync(cacheKey, "revoked", cacheOptions);
        }
        
        // Also mark in database for audit purposes
        await _userTokenRepository.RevokeJwtByIdAsync(jwtId);
        
        _logger.LogInformation("Revoked JWT {JwtId} with remaining lifetime {RemainingLifetime}", 
            jwtId, remainingLifetime);
    }
    
    public async Task RevokeAllUserRefreshTokensAsync(Guid userId)
    {
        var revokedCount = await _userTokenRepository.RevokeAllUserTokensOfTypeAsync(userId, "RefreshToken");
        _logger.LogInformation("Revoked {Count} refresh tokens for user {UserId}", revokedCount, userId);
    }
    
    public async Task RevokeDeviceRefreshTokensAsync(Guid userId, string deviceFingerprint)
    {
        var revokedCount = await _userTokenRepository.RevokeDeviceRefreshTokensAsync(userId, deviceFingerprint);
        _logger.LogInformation("Revoked {Count} refresh tokens for user {UserId} on device {DeviceFingerprint}", 
            revokedCount, userId, deviceFingerprint);
    }
    
    public async Task<int> CleanupExpiredTokensAsync()
    {
        var expiredCount = await _userTokenRepository.CleanupExpiredTokensAsync(true);
        var idleCount = await _userTokenRepository.CleanupIdleTokensAsync(7, true);
        var oldRevokedCount = await _userTokenRepository.CleanupOldRevokedTokensAsync(30, true);
        
        var totalCleaned = expiredCount + idleCount + oldRevokedCount;
        
        _logger.LogInformation("Cleaned up {TotalCount} tokens: {ExpiredCount} expired, {IdleCount} idle, {OldRevokedCount} old revoked", 
            totalCleaned, expiredCount, idleCount, oldRevokedCount);
        
        return totalCleaned;
    }
    
    // ========================================================================
    // Private Helper Methods
    // ========================================================================
    
    private (SecurityKey SigningKey, string Algorithm) GetSigningCredentials()
    {
        // Try to get RSA key first (RS256/ES256)
        var rsaKeyPath = _configuration["Jwt:RsaPrivateKeyPath"];
        if (!string.IsNullOrEmpty(rsaKeyPath) && File.Exists(rsaKeyPath))
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(rsaKeyPath));
            return (new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        }
        
        // Fallback to HMAC (HS256) for development
        var hmacKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured");
        var keyBytes = Encoding.UTF8.GetBytes(hmacKey);
        return (new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
    }
    
    private  bool IsJwtRevoked(string jwtId)
    {
        if (_distributedCache == null || string.IsNullOrEmpty(jwtId))
            return false;
        
        var cacheKey = $"{JWT_DENY_LIST_KEY_PREFIX}{jwtId}";
        var revoked =  _distributedCache.GetString(cacheKey);
        return !string.IsNullOrEmpty(revoked);
    }
    
    private static string GenerateSecureToken(int length)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
    
    private static void AddRoleClaims(User user, ICollection<Claim> claims)
    {
        var userRoles = user.GetRoles();
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }
    }
}
