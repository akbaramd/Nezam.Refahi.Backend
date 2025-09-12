using Microsoft.Extensions.Caching.Memory;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Service for generating and validating one-time passwords (OTPs)
/// </summary>
public class OtpService : IOtpService
{
    private readonly IMemoryCache _cache;
    private readonly Random _random = new();

    public OtpService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<string> GenerateOtpAsync(string userId, string purpose, int expiryMinutes = 5)
    {
        // Generate a 6-digit OTP
        var otp = _random.Next(100000, 999999).ToString();
        
        // Create a unique cache key for this user and purpose
        var cacheKey = GetCacheKey(userId, purpose);
        
        // Store the OTP in the cache with the specified expiry time
        _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(expiryMinutes));
        
        return Task.FromResult(otp);
    }

    public Task<bool> ValidateOtpAsync(string userId, string otp, string purpose)
    {
        // Create the cache key
        var cacheKey = GetCacheKey(userId, purpose);
        
        // Try to get the stored OTP from the cache
        if (_cache.TryGetValue(cacheKey, out string? storedOtp))
        {
            // Check if the provided OTP matches the stored OTP
            var isValid = storedOtp == otp;
            
            // If valid, remove the OTP from the cache to prevent reuse
            if (isValid)
            {
                _cache.Remove(cacheKey);
            }
            
            return Task.FromResult(isValid);
        }
        
        // OTP not found or expired
        return Task.FromResult(false);
    }
    
    private static string GetCacheKey(string userId, string purpose)
    {
        return $"OTP_{userId}_{purpose}";
    }
}
