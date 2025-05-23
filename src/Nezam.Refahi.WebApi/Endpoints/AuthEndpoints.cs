using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Application.Features.Auth.Commands.SendOtp;
using Nezam.Refahi.Application.Features.Auth.Commands.VerifyOtp;
using Nezam.Refahi.Application.Features.Auth.Commands.CompleteRegistration;
using Nezam.Refahi.Application.Features.Auth.Queries.GetCurrentUser;

namespace Nezam.Refahi.WebApi.Endpoints;

/// <summary>
/// Extension methods for authentication-related endpoints
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Maps all authentication-related endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        // Send OTP endpoint
        authGroup.MapPost("/send-otp", async (
            [FromBody] SendOtpRequest request,
            [FromServices] IMediator mediator,
            HttpContext httpContext) =>
        {
            try
            {
                // Create the command from the request
                var command = new SendOtpCommand
                {
                    NationalCode = request.NationalCode,
                    Purpose = request.Purpose ?? "login"
                };
                
                // Send the command to the mediator
                var response = await mediator.Send(command);
                
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithName("SendOtp")
        .WithOpenApi()
        .WithDescription("Sends an OTP code to the provided phone number");

        // Verify OTP endpoint
        authGroup.MapPost("/verify-otp", async (
            [FromBody] VerifyOtpRequest request,
            [FromServices] IMediator mediator) =>
        {
            try
            {
                // Create the command from the request
                var command = new VerifyOtpCommand
                {
                  NationalCode = request.NationalCode,
                    OtpCode = request.OtpCode,
                    Purpose = request.Purpose ?? "login"
                };
                
                // Send the command to the mediator
                var response = await mediator.Send(command);
                
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithName("VerifyOtp")
        .WithOpenApi()
        .WithDescription("Verifies an OTP code for a user");

        // Complete Registration endpoint - requires authentication
        authGroup.MapPost("/complete-registration", async (
            [FromBody] CompleteRegistrationRequest request,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUserService,
            HttpContext httpContext) =>
        {
            try
            {
                // Check if user is authenticated
                if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
                {
                    return Results.Unauthorized();
                }

                // Create the command from the request and set the user ID from the token
                var command = new CompleteRegistrationCommand
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    NationalId = request.NationalId,
                    UserId = currentUserService.UserId.Value
                };
                
                // Send the command to the mediator
                var response = await mediator.Send(command);
                
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithName("CompleteRegistration")
        .WithOpenApi()
        .RequireAuthorization() // Require JWT authentication
        .WithDescription("Completes user registration with profile information");

        // Get Current User endpoint - requires authentication
        authGroup.MapGet("/me", async (
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUserService) =>
        {
            try
            {
                // Check if user is authenticated
                if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
                {
                    return Results.Unauthorized();
                }

                // Create the query
                var query = new GetCurrentUserQuery();
                
                // Send the query to the mediator
                var response = await mediator.Send(query);
                
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithName("GetCurrentUser")
        .WithOpenApi()
        .RequireAuthorization() // Require JWT authentication
        .WithDescription("Gets the current authenticated user's profile");

        return app;
    }
}

// Request DTOs
public record SendOtpRequest(string NationalCode, string? Purpose = null, string? DeviceId = null);
public record VerifyOtpRequest(string NationalCode, string OtpCode, string? Purpose = null);
public record CompleteRegistrationRequest(
    string FirstName,
    string LastName,
    string NationalId);
