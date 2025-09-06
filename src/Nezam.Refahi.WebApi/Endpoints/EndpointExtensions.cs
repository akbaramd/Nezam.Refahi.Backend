namespace Nezam.Refahi.WebApi.Endpoints;

/// <summary>
/// Extension methods for mapping all API endpoints
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Maps all API endpoints for the application
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        // Authentication endpoints are now handled by Identity.Presentation module
        // Settings endpoints are now handled by Settings.Presentation module
        
        // Add other endpoint mappings here as the application grows
        // app.MapUserEndpoints();
        // app.MapPaymentEndpoints();
        // etc.
        
        return app;
    }
}
