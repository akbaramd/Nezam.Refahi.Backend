using MCA.SharedKernel.Domain.Models;
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
using Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetPaymentsPaginated;
using Nezam.Refahi.Finance.Application.Queries.Bills;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Finance.Presentation.Extensions;

namespace Nezam.Refahi.Finance.Presentation.Endpoints;

public static class PaymentEndpoints
{
  public static WebApplication MapPaymentEndpoints(this WebApplication app)
  {
    // ───────────────────── Me · Payments (user-specific) ─────────────────────
    var meGroup = app.MapGroup("/api/me/finance/payments")
      .WithTags("Payments")
      .RequireAuthorization();

    // Me: Get Paginated Payments for Current User
    meGroup.MapGet("/paginated", async (
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? status,
        [FromQuery] string? search,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromServices] IMediator mediator,
        [FromServices] ICurrentUserService currentUser,
        CancellationToken ct) =>
      {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
          return Results.Unauthorized();

        PaymentStatus? paymentStatus = null;
        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<PaymentStatus>(status, true, out var parsedStatus))
        {
          paymentStatus = parsedStatus;
        }

        var query = new GetPaymentsPaginatedQuery
        {
          PageNumber = pageNumber <= 0 ? 1 : pageNumber,
          PageSize = pageSize <= 0 ? 10 : pageSize,
          Status = paymentStatus,
          Search = search,
          FromDate = fromDate,
          ToDate = toDate,
          ExternalUserId = currentUser.UserId.Value
        };

        var result = await mediator.Send(query, ct);
        return result.ToResult();
      })
      .WithName("Me_GetPaymentsPaginated")
      .Produces<ApplicationResult<PaginatedResult<PaymentDto>>>(200)
      .Produces(400)
      .Produces(401);

    // Me: Create Payment
    meGroup.MapPost("", async (
        [FromBody] CreatePaymentCommand command,
        [FromServices] IMediator mediator,
        [FromServices] ICurrentUserService currentUserService) =>
      {
        if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
          return Results.Unauthorized();

        var commandWithUser = command with { ExternalUserId = currentUserService.UserId.Value };
        var result = await mediator.Send(commandWithUser);

        return result.ToResult();
      })
      .WithName("Me_CreatePayment")
      .Produces<ApplicationResult<CreatePaymentResponse>>()
      .Produces(400)
      .Produces(401);

    // Me: Pay with Wallet
    meGroup.MapPost("/wallet", async (
        [FromBody] PayWithWalletCommand command,
        [FromServices] IMediator mediator,
        [FromServices] ICurrentUserService currentUserService) =>
      {
        if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
          return Results.Unauthorized();

        var result = await mediator.Send(command);
        return result.ToResult();
      })
      .WithName("Me_PayWithWallet")
      .Produces<ApplicationResult<PayWithWalletResponse>>()
      .Produces(400)
      .Produces(401);

    // Me: Get Payment Details (user's own payment)
    meGroup.MapGet("/{paymentId:guid}", async (
        [FromRoute] Guid paymentId,
        [FromServices] IMediator mediator,
        [FromServices] ICurrentUserService currentUser,
        CancellationToken ct) =>
      {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
          return Results.Unauthorized();

        var query = new GetPaymentDetailQuery 
        { 
          PaymentId = paymentId,
          ExternalUserId = currentUser.UserId.Value
        };
        var result = await mediator.Send(query, ct);
        return result.ToResult();
      })
      .WithName("Me_GetPaymentDetail")
      .Produces<ApplicationResult<PaymentDetailDto>>(200)
      .Produces(400)
      .Produces(401);

    // ───────────────────── Global Payment Endpoints ─────────────────────
    var group = app.MapGroup("/api/v1/payments")
      .WithTags("Payments");

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

    // Global: Get Supported Gateways
    group.MapGet("/gateways", async (
        [FromServices] IPaymentService paymentService) =>
      {
        var gateways = await paymentService.GetSupportedGatewaysAsync();
        return Results.Ok(gateways);
      })
      .RequireAuthorization()
      .WithName("GetSupportedPaymentGateways")
      .Produces<IEnumerable<PaymentGatewayInfo>>();

    // ═══════════════════════ BILL MANAGEMENT ENDPOINTS ═══════════════════════
    if (app.Environment.IsDevelopment())
    {
      group.MapPost("complete", async (
          [FromBody] CompletePaymentCommand command,
          [FromServices] IMediator mediator) =>
        {
          var result = await mediator.Send(command);
          return result.ToResult();
        })
        .AllowAnonymous()
        .WithName("CompletePayment")
        .Produces<ApplicationResult<CreatePaymentResponse>>()
        .Produces(400);
    }

    return app;
  }
}
