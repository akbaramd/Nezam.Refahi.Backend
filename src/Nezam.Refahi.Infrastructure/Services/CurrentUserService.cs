using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Nezam.Refahi.Application.Common.Interfaces;

namespace Nezam.Refahi.Infrastructure.Services;

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
