using Hangfire.Dashboard;

namespace Nezam.Refahi.WebApi.Services;

/// <summary>
/// Authorization filter for Hangfire Dashboard
/// In production, this should be replaced with proper authentication/authorization
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // For development purposes, allow all access
        // In production, implement proper authentication/authorization
        // Example: Check if user is authenticated and has admin role
        return true;
    }
}
