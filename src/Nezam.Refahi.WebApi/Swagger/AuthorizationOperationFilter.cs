using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Nezam.Refahi.WebApi.Swagger;

/// <summary>
/// Operation filter to show authorization icon only for endpoints that require authentication
/// and hide it for anonymous endpoints. Supports both Controller attributes and Minimal API RequireAuthorization().
/// </summary>
public class AuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Get the method info
        var methodInfo = context.MethodInfo;
        var controllerType = methodInfo.DeclaringType;
        
        // Check for Minimal API RequireAuthorization() metadata
        var hasRequireAuthorization = HasRequireAuthorizationMetadata(context);
        
        // Check if the endpoint has [AllowAnonymous] attribute
        var hasAllowAnonymous = methodInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null;
        
        // Check if the endpoint has [Authorize] attribute
        var hasAuthorize = methodInfo.GetCustomAttribute<AuthorizeAttribute>() != null;
        
        // Check if the controller has [Authorize] attribute
        var controllerHasAuthorize = controllerType?.GetCustomAttribute<AuthorizeAttribute>() != null;
        
        // Check if the controller has [AllowAnonymous] attribute
        var controllerHasAllowAnonymous = controllerType?.GetCustomAttribute<AllowAnonymousAttribute>() != null;
        
        // Determine if this endpoint requires authorization
        // Priority: Method-level attributes override controller-level attributes and Minimal API metadata
        var requiresAuth = false;
        
        if (hasAllowAnonymous)
        {
            // Method explicitly allows anonymous access
            requiresAuth = false;
        }
        else if (hasAuthorize)
        {
            // Method explicitly requires authorization
            requiresAuth = true;
        }
        else if (hasRequireAuthorization)
        {
            // Minimal API requires authorization
            requiresAuth = true;
        }
        else if (controllerHasAllowAnonymous)
        {
            // Controller allows anonymous access by default
            requiresAuth = false;
        }
        else if (controllerHasAuthorize)
        {
            // Controller requires authorization by default
            requiresAuth = true;
        }
        // If no attributes or metadata are found, assume no authorization required
        
        // Apply security requirements based on authorization status
        if (requiresAuth)
        {
            // Add security requirement for authorized endpoints
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                }
            };
            
            // Add a note to the operation description
            if (string.IsNullOrEmpty(operation.Description))
            {
                operation.Description = "🔒 This endpoint requires authentication.";
            }
            else
            {
                operation.Description = $"🔒 {operation.Description}";
            }
        }
        else
        {
            // For anonymous endpoints, ensure no security requirements are added
            operation.Security = new List<OpenApiSecurityRequirement>();
            
            // Add a note to indicate this is a public endpoint
            if (string.IsNullOrEmpty(operation.Description))
            {
                operation.Description = "🌐 This endpoint is publicly accessible.";
            }
            else
            {
                operation.Description = $"🌐 {operation.Description}";
            }
        }
    }
    
    /// <summary>
    /// Checks if the endpoint has RequireAuthorization() metadata from Minimal APIs
    /// </summary>
    private bool HasRequireAuthorizationMetadata(OperationFilterContext context)
    {
        // Check if the endpoint has RequireAuthorization metadata
        // This is added by the .RequireAuthorization() method in Minimal APIs
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        
        // Look for authorization metadata
        foreach (var metadata in endpointMetadata)
        {
            // Check for RequireAuthorization metadata
            if (metadata is AuthorizeAttribute)
            {
                return true;
            }
        }
        
        return false;
    }
}
