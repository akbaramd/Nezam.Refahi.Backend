// -----------------------------------------------------------------------------
// Identity.Presentation/Endpoints/IdentityEndpoints.cs
// -----------------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Identity.Application.Features.Authentication.Commands.Logout;
using Nezam.Refahi.Identity.Application.Features.Authentication.Commands.RefreshToken;
using Nezam.Refahi.Identity.Application.Features.Authentication.Commands.SendOtp;
using Nezam.Refahi.Identity.Application.Features.Authentication.Commands.VerifyOtp;
using Nezam.Refahi.Identity.Application.Features.Authentication.Queries.GetCurrentUser;
using Nezam.Refahi.Identity.Presentation.Models;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

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

            // Validate scope
            var scope = request.Scope ?? "app";
            if (!IsValidScope(scope))
                return Results.BadRequest("Invalid scope. Allowed values: panel, app");

            var cmd = new SendOtpCommand
            {
                NationalCode = request.NationalCode,
                Purpose = request.Purpose ?? "login",
                DeviceId = deviceId,
                IpAddress = ipAddress,
                Scope = scope
            };
            var res = await mediator.Send(cmd);
            return Results.Ok(res);
        })
        .WithName("SendOtp")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Send OTP Code",
            Description = "Sends an OTP (One-Time Password) code to the user's phone number for authentication purposes",
            Tags = new List<OpenApiTag> { new() { Name = "Authentication" } }
        })
        .Produces<ApplicationResult<SendOtpResponse>>(StatusCodes.Status200OK, "application/json")
        ;

        // ───────────────────── 2) OTP Verification ───────────
        authGroup.MapPost("/otp/verify", async (
            [FromBody] VerifyOtpRequest request,
            [FromServices] IMediator mediator,
            HttpContext httpContext) =>
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var deviceId  = request.DeviceId ??
                            httpContext.Request.Headers["X-Device-Id"].FirstOrDefault() ?? "unknown";

            // Validate scope
            var scope = request.Scope ?? "app";
            if (!IsValidScope(scope))
                return Results.BadRequest("Invalid scope. Allowed values: panel, app");

            var cmd = new VerifyOtpCommand
            {
                ChallengeId = Guid.Parse(request.ChallengeId),
                OtpCode      = request.OtpCode,
                Purpose      = request.Purpose ?? "login",
                DeviceId     = deviceId,
                IpAddress    = ipAddress,
                Scope        = scope
            };
            return Results.Ok(await mediator.Send(cmd));
        })
        .WithName("VerifyOtp")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Verify OTP Code",
            Description = "Verifies an OTP code and returns authentication tokens if successful",
            Tags = new List<OpenApiTag> { new() { Name = "Authentication" } }
        })
        .Produces<ApplicationResult<VerifyOtpResponse>>(StatusCodes.Status200OK, "application/json");

       
// ───────────────────── 2) OTP Verification ───────────
        authGroup.MapPost("/refresh", async (
            [FromBody] RefreshTokenRequest request,
            [FromServices] IMediator mediator,
            HttpContext httpContext) =>
          {
           
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var deviceId  = request.DeviceId ??
                            httpContext.Request.Headers["X-Device-Id"].FirstOrDefault() ?? "unknown";
            var cmd = new RefreshTokenCommand()
            {
              RefreshToken = request.RefreshToken,
              IpAddress = ipAddress,
              UserAgent = "",
              DeviceFingerprint = deviceId,
            };
            return Results.Ok(await mediator.Send(cmd));
          })
          .WithName("RefreshToken")
          .WithOpenApi(operation => new(operation)
          {
            Summary = "RefreshToken ",
            Description = "RefreshToken an OTP code and returns authentication tokens if successful",
            Tags = new List<OpenApiTag> { new() { Name = "Authentication" } }
          })
          .Produces<ApplicationResult<RefreshTokenResponse>>(StatusCodes.Status200OK, "application/json");

        // ───────────────────── 4) Current UserDetail Profile ─────────────────────
        authGroup.MapGet("/profile", async (
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService current) =>
        {
            if (!current.IsAuthenticated) return Results.Unauthorized();
            return Results.Ok(await mediator.Send(new GetCurrentUserQuery()));
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get Current UserDetail Profile",
            Description = "Retrieves the profile information of the currently authenticated user",
            Tags = new List<OpenApiTag> { new() { Name = "Authentication" } }
        })
        .Produces<ApplicationResult<CurrentUserResponse>>(StatusCodes.Status200OK, "application/json");

        // ───────────────────── 5) UserDetail Session Management ─────
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
        .WithOpenApi(operation => new(operation)
        {
            Summary = "UserDetail Logout",
            Description = "Logs out the user and revokes both access and refresh tokens",
            Tags = new List<OpenApiTag> { new() { Name = "Authentication" } }
        })
        .Produces<ApplicationResult<LogoutResponse>>(StatusCodes.Status200OK, "application/json");

        return app;
    }

    /// <summary>
    /// Validates if the provided scope is valid
    /// </summary>
    private static bool IsValidScope(string scope)
    {
        return scope.ToLowerInvariant() is "panel" or "app";
    }
}

// ─────────────────────────── DTOs ───────────────────────────

// اختیاری؛ Access از هدر خوانده می‌شود
