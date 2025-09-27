using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nezam.Refahi.Finance.Application.Commands.Bills;
using Nezam.Refahi.Finance.Application.Queries.Bills;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Presentation.Endpoints;

public static class BillEndpoints
{
    public static WebApplication MapBillEndpoints(this WebApplication app)
    {
        var billGroup = app.MapGroup("/api/v1/bills")
            .WithTags("Bills")
            .RequireAuthorization();

        // ───────────────────── Create Bill ─────────────────────
        billGroup.MapPost("/", async (
                [FromBody] CreateBillCommand command,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Created($"/api/v1/bills/{result.Data!.BillId}", result) : Results.BadRequest(result);
            })
            .WithName("CreateBill")
            .Produces<ApplicationResult<CreateBillResponse>>(201)
            .Produces(400);

        // ───────────────────── Get User Bills ─────────────────────
            billGroup.MapGet("/user/{externalUserId}", async (
                [FromServices] IMediator mediator,
                Guid externalUserId,
                [FromQuery] string? status = null,
                [FromQuery] string? billType = null,
                [FromQuery] bool onlyOverdue = false,
                [FromQuery] bool onlyUnpaid = false,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 20,
                [FromQuery] string sortBy = "IssueDate",
                [FromQuery] string sortDirection = "desc") =>
            {
                var query = new GetUserBillsQuery
                {
                    ExternalUserId = externalUserId,
                    Status = status,
                    BillType = billType,
                    OnlyOverdue = onlyOverdue,
                    OnlyUnpaid = onlyUnpaid,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                };

                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetUserBills")
            .Produces<ApplicationResult<UserBillsResponse>>()
            .Produces(400);

        // ───────────────────── Get Bill Payment Status (By ID) ─────────────────────
        billGroup.MapGet("/{billId:guid}/status", async (
                Guid billId,
                [FromServices] IMediator mediator,
                [FromQuery] bool includePaymentHistory = false,
                [FromQuery] bool includeRefundHistory = false,
                [FromQuery] bool includeBillItems = false) =>
            {
                var query = new GetBillPaymentStatusQuery
                {
                    BillId = billId,
                    IncludePaymentHistory = includePaymentHistory,
                    IncludeRefundHistory = includeRefundHistory,
                    IncludeBillItems = includeBillItems
                };

                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetBillPaymentStatus")
            .Produces<ApplicationResult<BillPaymentStatusResponse>>()
            .Produces(400);

        // ───────────────────── Get Bill Payment Status (By Number) ─────────────────────
        billGroup.MapGet("/number/{billNumber}/status", async (
                string billNumber,
                [FromServices] IMediator mediator,
                [FromQuery] bool includePaymentHistory = false,
                [FromQuery] bool includeRefundHistory = false,
                [FromQuery] bool includeBillItems = false) =>
            {
                var query = new GetBillPaymentStatusByNumberQuery
                {
                    BillNumber = billNumber,
                    IncludePaymentHistory = includePaymentHistory,
                    IncludeRefundHistory = includeRefundHistory,
                    IncludeBillItems = includeBillItems
                };

                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetBillPaymentStatusByNumber")
            .Produces<ApplicationResult<BillPaymentStatusResponse>>()
            .Produces(400);

        // ───────────────────── Get Bill Payment Status (By Tracking Code) ─────────────────────
        billGroup.MapGet("/tracking/{trackingCode}/status", async (
                string trackingCode,
                [FromServices] IMediator mediator,
                [FromQuery] string billType = "WalletDeposit",
                [FromQuery] bool includePaymentHistory = false,
                [FromQuery] bool includeRefundHistory = false,
                [FromQuery] bool includeBillItems = false) =>
            {
                var query = new GetBillPaymentStatusByTrackingCodeQuery
                {
                    TrackingCode = trackingCode,
                    BillType = billType,
                    IncludePaymentHistory = includePaymentHistory,
                    IncludeRefundHistory = includeRefundHistory,
                    IncludeBillItems = includeBillItems
                };

                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetBillPaymentStatusByTrackingCode")
            .Produces<ApplicationResult<BillPaymentStatusResponse>>()
            .Produces(400);

        // ───────────────────── Issue Bill ─────────────────────
        billGroup.MapPost("/{billId:guid}/issue", async (
                Guid billId,
                [FromServices] IMediator mediator) =>
            {
                var command = new IssueBillCommand { BillId = billId };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("IssueBill")
            .Produces<ApplicationResult<IssueBillResponse>>()
            .Produces(400);

        // ───────────────────── Cancel Bill ─────────────────────
        billGroup.MapPost("/{billId:guid}/cancel", async (
                Guid billId,
                [FromBody] CancelBillRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new CancelBillCommand
                {
                    BillId = billId,
                    Reason = request.Reason
                };

                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("CancelBill")
            .Produces<ApplicationResult<CancelBillResponse>>()
            .Produces(400);

        return app;
    }
}

public record AddBillItemRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal UnitPriceRials { get; init; }
    public int Quantity { get; init; } = 1;
    public decimal? DiscountPercentage { get; init; }
}

public record CancelBillRequest
{
    public string? Reason { get; init; }
}
