// -----------------------------------------------------------------------------
// Identity.Presentation/Endpoints/IdentityEndpoints.cs
// -----------------------------------------------------------------------------

using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nezam.Refahi.Identity.Application.Commands.CompleteRegistration;
using Nezam.Refahi.Identity.Application.Commands.Logout;
using Nezam.Refahi.Identity.Application.Commands.SendOtp;
using Nezam.Refahi.Identity.Application.Commands.VerifyOtp;
using Nezam.Refahi.Identity.Application.Queries.GetCurrentUser;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Identity.Presentation.Endpoints;

public static class IdentityEndpoints
{
    public static WebApplication MapIdentityEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/api/v1/auth").WithTags("Authentication");

        // ───────────────────── 1) OTP Operations ──────────────────────────────
        authGroup.MapPost("/otp", async (
            [FromBody] SendOtpRequest request,
            [FromServices] IMediator mediator,
            HttpContext httpContext) =>
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var deviceId = request.DeviceId ??
                          httpContext.Request.Headers["X-Device-Id"].FirstOrDefault() ?? "unknown";

            var cmd = new SendOtpCommand
            {
                NationalCode = request.NationalCode,
                Purpose = request.Purpose ?? "login",
                DeviceId = deviceId,
                IpAddress = ipAddress
            };
            return Results.Ok(await mediator.Send(cmd));
        })
        .WithName("SendOtp")
        .WithOpenApi()
        .WithDescription("Sends an OTP code to the provided phone number");

        // ───────────────────── 2) OTP Verification ───────────
        authGroup.MapPost("/otp/verify", async (
            [FromBody] VerifyOtpRequest request,
            [FromServices] IMediator mediator,
            HttpContext httpContext) =>
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var deviceId  = request.DeviceId ??
                            httpContext.Request.Headers["X-Device-Id"].FirstOrDefault() ?? "unknown";

            var cmd = new VerifyOtpCommand
            {
                NationalCode = request.NationalCode,
                OtpCode      = request.OtpCode,
                Purpose      = request.Purpose ?? "login",
                DeviceId     = deviceId,
                IpAddress    = ipAddress
            };
            return Results.Ok(await mediator.Send(cmd));
        })
        .WithName("VerifyOtp")
        .WithOpenApi()
        .WithDescription("Verifies an OTP code for a user");

        // ───────────────────── 3) User Registration ─────────────────
        authGroup.MapPost("/registration", async (
            [FromBody] CompleteRegistrationRequest request,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService current) =>
        {
            if (!current.IsAuthenticated || !current.UserId.HasValue)
                return Results.Unauthorized();

            var cmd = new CompleteRegistrationCommand
            {
                UserId     = current.UserId.Value,
                FirstName  = request.FirstName,
                LastName   = request.LastName,
                NationalId = request.NationalId
            };
            return Results.Ok(await mediator.Send(cmd));
        })
        .RequireAuthorization()
        .WithName("CompleteRegistration")
        .WithOpenApi()
        .WithDescription("Completes user registration with profile information");

        // ───────────────────── 4) Current User Profile ─────────────────────
        authGroup.MapGet("/profile", async (
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService current) =>
        {
            if (!current.IsAuthenticated) return Results.Unauthorized();
            return Results.Ok(await mediator.Send(new GetCurrentUserQuery()));
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser")
        .WithOpenApi()
        .WithDescription("Gets the current authenticated user's profile");

        // ───────────────────── 5) User Session Management ─────
        authGroup.MapPost("/logout", async (
            [FromBody] LogoutRequest body,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService current,
            HttpContext httpContext) =>
        {
            if (!current.IsAuthenticated || !current.UserId.HasValue)
                return Results.Unauthorized();

            // Access-Token (Bearer …) از هِدر Authorization
            var bearer = httpContext.Request.Headers.Authorization.ToString();
            var accessToken = bearer.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                              ? bearer["Bearer ".Length..]
                              : bearer;

            var cmd = new LogoutCommand(
                current.UserId.Value,
                accessToken,
                body.RefreshToken);

            var result = await mediator.Send(cmd);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .RequireAuthorization()
        .WithName("Logout")
        .WithOpenApi()
        .WithDescription("Logs the user out and revokes tokens");

        return app;
    }
}

// ─────────────────────────── DTOs ───────────────────────────
public record SendOtpRequest(string NationalCode, string? Purpose = null, string? DeviceId = null);

public record VerifyOtpRequest(
    string NationalCode,
    string OtpCode,
    [property: JsonIgnore]
    string? Purpose  = null,
    [property: JsonIgnore]
    string? DeviceId = null); // IpAddress از HttpContext استخراج می‌شود

public record CompleteRegistrationRequest(
    string FirstName,
    string LastName,
    string NationalId);

public record LogoutRequest(string? RefreshToken);  // اختیاری؛ Access از هدر خوانده می‌شود
