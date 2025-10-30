using System.Security.Claims;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Shared.Infrastructure.Services;

/// <summary>
/// Service to access the current authenticated user's information from the HTTP context
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            
            return null;
        }
    }

    public string? UserNationalNumber
    {
        get
        {
            var nationalNumberClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("national_id");
            var result = nationalNumberClaim?.Value;
            
            // Debug logging
            if (string.IsNullOrWhiteSpace(result))
            {
                var allClaims = _httpContextAccessor.HttpContext?.User?.Claims?.Select(c => $"{c.Type}={c.Value}").ToList();
                System.Diagnostics.Debug.WriteLine($"UserNationalNumber is null/empty. Available claims: {string.Join(", ", allClaims ?? new List<string>())}");
            }
            
            return result;
        }
    }

    public string? UserFullName
    {
        get
        {
            var fullNameClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name);
            return fullNameClaim?.Value;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles
    {
        get
        {
            var roleClaims = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
            return roleClaims?.Select(c => c.Value) ?? Array.Empty<string>();
        }
    }

    public bool HasRole(string role)
    {
        return Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
    }
}
