using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Finance.Application.Features.Payments.Commands.HandleCallback;
using Nezam.Refahi.Finance.Application.Configuration;
using Nezam.Refahi.Finance.Application.Commands.Bills;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetPaymentDetail;
using Nezam.Refahi.Finance.Application.Queries.Bills;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Presentation.Endpoints;

public static class PaymentEndpoints
{
  public static WebApplication MapPaymentEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/v1/payments")
      .WithTags("Payments");

    // ───────────────────── Get Payment Details ─────────────────────
    group.MapGet("/{paymentId:guid}", async (
        [FromRoute] Guid paymentId,
        [FromServices] IMediator mediator) =>
      {
        var query = new GetPaymentDetailQuery { PaymentId = paymentId };

        var result = await mediator.Send(query);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
      })
      .RequireAuthorization()
      .WithName("GetPaymentDetail")
      .Produces<ApplicationResult<PaymentDetailDto>>(200)
      .Produces(400)
      .WithOpenApi(op => new(op)
      {
        Summary = "Get payment details by ID",
        Description =
          "Returns detailed information about a specific payment (PaymentDetailDto) including status, amount, method, and metadata.",
        Tags = new List<OpenApiTag> { new() { Name = "Payments" } }
      });
    // ───────────────────── Payment Callback (GET + POST, Anonymous) ─────────────────────
    group.MapMethods("/callback", new[] { "GET", "POST" }, async (
        [FromServices] IMediator mediator,
        [FromServices] IOptions<FrontendSettings> frontendSettings) =>
      {
        var result = await mediator.Send(new HandlePaymentCallbackCommand());
        var frontend = frontendSettings.Value;

        if (result.IsSuccess && result.Data != null)
        {
          var callbackResult = result.Data;

          // Determine target redirect (success or failure)
          string redirectUrl = callbackResult.IsSuccessful
            ? frontend.GetPaymentSuccessUrl(callbackResult.BillId, callbackResult.PaymentId,
              callbackResult.BillTrackingCode, callbackResult.BillType)
            : frontend.GetPaymentFailureUrl(callbackResult.BillId, callbackResult.PaymentId,
              callbackResult.BillTrackingCode, callbackResult.BillType);

          // Normalize redirect URL
          redirectUrl = NormalizeEncodedInternalPath(redirectUrl, frontend.GenericPaymentFailurePath);

          return Results.Redirect(redirectUrl);
        }

        // Generic fallback
        return Results.Redirect(frontend.GenericPaymentFailurePath);
      })
      .AllowAnonymous()
      .WithName("PaymentCallback")
      .Produces<ApplicationResult<PaymentCallbackResult>>()
      .Produces(302)
      .Produces(400);

// ───────────────────── Helper: Normalize Redirect URL ─────────────────────
    static string NormalizeEncodedInternalPath(string encoded, string fallback)
    {
      if (string.IsNullOrWhiteSpace(encoded))
        return fallback;

      // Decode %2F → /
      string decoded = Uri.UnescapeDataString(encoded).Trim();

      // If still double-encoded (e.g. "%252Fbills%252F..."), decode again
      if (decoded.Contains("%2F"))
        decoded = Uri.UnescapeDataString(decoded);




      return decoded;
    }

    // ───────────────────── Payment Success Page ─────────────────────
    group.MapGet("/success", () =>
      {
        // This could return a view or redirect to frontend success page
        return Results.Content(@"
                    <html>
                        <body>
                            <h1>پرداخت موفق</h1>
                            <p>پرداخت شما با موفقیت انجام شد.</p>
                            <script>
                                setTimeout(() => {
                                    window.close();
                                }, 3000);
                            </script>
                        </body>
                    </html>", "text/html; charset=utf-8");
      })
      .AllowAnonymous()
      .WithName("PaymentSuccess");

    // ───────────────────── Payment Failed Page ─────────────────────
    group.MapGet("/failed", () =>
      {
        // This could return a view or redirect to frontend failure page
        return Results.Content(@"
                    <html>
                        <body>
                            <h1>پرداخت ناموفق</h1>
                            <p>پرداخت شما ناموفق بود. لطفاً دوباره تلاش کنید.</p>
                            <script>
                                setTimeout(() => {
                                    window.close();
                                }, 3000);
                            </script>
                        </body>
                    </html>", "text/html; charset=utf-8");
      })
      .AllowAnonymous()
      .WithName("PaymentFailed");

    // ───────────────────── Get Supported Gateways ─────────────────────
    group.MapGet("/gateways", async (
        [FromServices] IPaymentService paymentService) =>
      {
        var gateways = await paymentService.GetSupportedGatewaysAsync();
        return Results.Ok(gateways);
      })
      .RequireAuthorization()
      .WithName("GetSupportedPaymentGateways")
      .Produces<IEnumerable<PaymentGatewayInfo>>();

    // ───────────────────── Create Payment ─────────────────────
    group.MapPost("", async (
        [FromBody] CreatePaymentCommand command,
        [FromServices] IMediator mediator,
        [FromServices] ICurrentUserService currentUserService) =>
      {
        if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
        {
          return Results.Unauthorized();
        }

        // Set the external user ID from the current user service
        var commandWithUser = command with { ExternalUserId = currentUserService.UserId.Value };
        var result = await mediator.Send(commandWithUser);

        if (result.IsSuccess)
        {
          return Results.Ok(result);
        }

        return Results.BadRequest(result);
      })
      .RequireAuthorization()
      .WithName("CreatePayment")
      .Produces<ApplicationResult<CreatePaymentResponse>>()
      .Produces(400)
      .Produces(401);

    // ───────────────────── Pay with Wallet ─────────────────────
    group.MapPost("/wallet", async (
        [FromBody] PayWithWalletCommand command,
        [FromServices] IMediator mediator) =>
      {
        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
          return Results.Ok(result);
        }

        return Results.BadRequest(result);
      })
      .RequireAuthorization()
      .WithName("PayWithWallet")
      .Produces<ApplicationResult<PayWithWalletResponse>>()
      .Produces(400);

    // ═══════════════════════ BILL MANAGEMENT ENDPOINTS ═══════════════════════
    if (app.Environment.IsDevelopment())
    {
      group.MapPost("complete", async (
          [FromBody] CompletePaymentCommand command,
          [FromServices] IMediator mediator) =>
        {
          var result = await mediator.Send(command);

          if (result.IsSuccess)
          {
            return Results.Ok(result);
          }

          return Results.BadRequest(result);
        })
        .AllowAnonymous()
        .WithName("CompletePayment")
        .Produces<ApplicationResult<CreatePaymentResponse>>()
        .Produces(400);
    }

    return app;
  }
}
