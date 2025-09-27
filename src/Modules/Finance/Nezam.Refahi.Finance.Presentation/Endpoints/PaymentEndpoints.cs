using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Finance.Application.Features.Payments.Commands.HandleCallback;
using Nezam.Refahi.Finance.Application.Configuration;
using Nezam.Refahi.Finance.Application.Commands.Bills;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Queries.Bills;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Presentation.Endpoints;

public static class PaymentEndpoints
{
    public static WebApplication MapPaymentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/payments")
            .WithTags("Payments");

        
        // ───────────────────── Payment Callback (Anonymous) ─────────────────────
        group.MapGet("/callback", async (
                [FromServices] IMediator mediator,
                [FromServices] IOptions<FrontendSettings> frontendSettings) =>
            {
                var command = new HandlePaymentCallbackCommand();
                var result = await mediator.Send(command);

                if (result.IsSuccess && result.Data != null)
                {
                    var callbackResult = result.Data;
                    
                    // Always redirect to the specified URL
                    if (!string.IsNullOrEmpty(callbackResult.RedirectUrl))
                    {
                        return Results.Redirect(callbackResult.RedirectUrl);
                    }
                    
                    // Fallback redirect if no URL provided
                    if (callbackResult.IsSuccessful)
                    {
                        return Results.Redirect(frontendSettings.Value.GetPaymentSuccessUrl(callbackResult.BillId));
                    }
                    else
                    {
                        return Results.Redirect(frontendSettings.Value.GetPaymentFailureUrl(callbackResult.BillId));
                    }
                }
                else
                {
                    // Return error response with redirect to failure page
                    // Try to get BillId from result if available
                    if (result.Data?.BillId != null && result.Data.BillId != Guid.Empty)
                    {
                        return Results.Redirect(frontendSettings.Value.GetPaymentFailureUrl(result.Data.BillId));
                    }
                    return Results.Redirect(frontendSettings.Value.GetGenericPaymentFailureUrl());
                }
            })
            .AllowAnonymous() // Payment callbacks should be anonymous
            .WithName("PaymentCallback")
            .Produces<ApplicationResult<PaymentCallbackResult>>()
            .Produces(302) // Redirect
            .Produces(400);

        // ───────────────────── Payment Callback (POST) ─────────────────────
        group.MapPost("/callback", async (
                [FromServices] IMediator mediator,
                [FromServices] IOptions<FrontendSettings> frontendSettings) =>
            {
                var command = new HandlePaymentCallbackCommand();
                var result = await mediator.Send(command);

                if (result.IsSuccess && result.Data != null)
                {
                    var callbackResult = result.Data;
                    
                    // Always redirect to the specified URL
                    if (!string.IsNullOrEmpty(callbackResult.RedirectUrl))
                    {
                        return Results.Redirect(callbackResult.RedirectUrl);
                    }
                    
                    // Fallback redirect if no URL provided
                    if (callbackResult.IsSuccessful)
                    {
                        return Results.Redirect(frontendSettings.Value.GetPaymentSuccessUrl(callbackResult.BillId));
                    }
                    else
                    {
                        return Results.Redirect(frontendSettings.Value.GetPaymentFailureUrl(callbackResult.BillId));
                    }
                }
                else
                {
                    // Return error response with redirect to failure page
                    // Try to get BillId from result if available
                    if (result.Data?.BillId != null && result.Data.BillId != Guid.Empty)
                    {
                        return Results.Redirect(frontendSettings.Value.GetPaymentFailureUrl(result.Data.BillId));
                    }
                    return Results.Redirect(frontendSettings.Value.GetGenericPaymentFailureUrl());
                }
            })
            .AllowAnonymous() // Payment callbacks should be anonymous
            .WithName("PaymentCallbackPost")
            .Produces<ApplicationResult<PaymentCallbackResult>>()
            .Produces(302) // Redirect
            .Produces(400);

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
            .WithName("CreatePayment")
            .Produces<ApplicationResult<CreatePaymentResponse>>()
            .Produces(400);

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
