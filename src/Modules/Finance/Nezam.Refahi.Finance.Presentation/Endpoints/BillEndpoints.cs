using MCA.SharedKernel.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Finance.Application.Commands.Bills;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetBillPayments;
using Nezam.Refahi.Finance.Application.Queries.Bills;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Presentation.Endpoints;

public static class BillEndpoints
{
    public static WebApplication MapBillEndpoints(this WebApplication app)
    {
        // ========================= /api/v1/me/bills =========================
        var billMeGroup = app.MapGroup("/api/v1/me/bills")
            .WithTags("Bills")
            .RequireAuthorization();

        // GET /api/v1/me/bills
        billMeGroup.MapGet("", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] string? status = null,
                [FromQuery] string? billType = null,
                [FromQuery] string? searchTerm = null,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 20,
                [FromQuery] string sortBy = "IssueDate",
                [FromQuery] string sortDirection = "desc") =>
            {
                var query = new ListUserBillsByUserIdQuery
                {
                    ExternalUserId = currentUserService.GetRequiredUserId(),
                    Status = status,
                    BillType = billType,
                    SearchTerm = searchTerm,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                };

                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetMyBills")
            .Produces<ApplicationResult<PaginatedResult<BillDto>>>(200)
            .Produces(400)
            .WithOpenApi(op => new(op)
            {
                Summary = "List current user's bills (paginated)",
                Description = "Returns a paginated list of BillDto for the current user with filtering and sorting.",
                Tags = new List<OpenApiTag> { new() { Name = "Bills" } }
            });
      
        billMeGroup.MapGet("/{billId:guid}/discount-codes/{code}/validation", async (
            [FromRoute] Guid billId,
            [FromRoute] string code,
            [FromServices] ICurrentUserService currentUserService,
            [FromServices] IMediator mediator) =>
          {
            var externalUserId = currentUserService.GetRequiredUserId();
            if (externalUserId == Guid.Empty)
              return Results.BadRequest(ApplicationResult<DiscountValidationDto>.Failure("externalUserId is required."));

            var query = new ValidateDiscountCodeQuery
            {
              BillId = billId,
              DiscountCode = code,
              ExternalUserId = externalUserId
            };

            var result = await mediator.Send(query);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
          })
          .WithName("ValidateDiscountCodeForUser")
          .Produces<ApplicationResult<DiscountValidationDto>>(200)
          .Produces(400)
          .WithOpenApi(op => new(op)
          {
            Summary = "Validate a discount code for a specific user's bill",
            Description = "Admin/operator variant. Requires explicit externalUserId.",
            Tags = new List<OpenApiTag> { new() { Name = "Discount Codes" } }
          });
        // ========================= /api/v1/bills =========================
        var billGroup = app.MapGroup("/api/v1/bills")
            .WithTags("Bills")
            .RequireAuthorization();

        // ---------- Bill Details (canonical) ----------
        // GET /api/v1/bills/{billId}
        billGroup.MapGet("/{billId:guid}", async (
                Guid billId,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(new GetBillDetailsByIdQuery()
                {
                  BillId = billId
                });
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetBillDetailsById")
            .Produces<ApplicationResult<BillDetailDto>>(200)
            .Produces(400)
            .WithOpenApi(op => new(op)
            {
                Summary = "Get bill details by Id",
                Description = "Returns BillDetailDto including items, payments, and refunds.",
                Tags = new List<OpenApiTag> { new() { Name = "Bills" } }
            });
// GET /api/v1/bills/{billId}/payments
        billGroup.MapGet("/{billId:guid}/payments", async (
            [FromRoute] Guid billId,
            [FromQuery] string? searchTerm,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] string sortBy,
            [FromQuery] string sortDirection,
            [FromServices] IMediator mediator) =>
          {
            var query = new GetBillPaymentsQuery
            {
              BillId = billId,
              SearchTerm = searchTerm,
              PageNumber = pageNumber > 0 ? pageNumber : 1,
              PageSize = pageSize > 0 ? pageSize : 20,
              SortBy = string.IsNullOrWhiteSpace(sortBy) ? "CreatedAt" : sortBy,
              SortDirection = string.IsNullOrWhiteSpace(sortDirection) ? "desc" : sortDirection
            };

            var result = await mediator.Send(query);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
          })
          .WithName("GetBillPayments")
          .Produces<ApplicationResult<PaginatedResult<PaymentDto>>>(200)
          .Produces(400)
          .WithOpenApi(op => new(op)
          {
            Summary = "Get payments for a specific bill",
            Description = "Returns a paginated list of payments (PaymentDto) associated with a given bill ID. Supports search, sorting, and pagination.",
            Tags = new List<OpenApiTag> { new() { Name = "Payments" } }
          });
        // GET /api/v1/bills/by-number/{billNumber}
        billGroup.MapGet("/by-number/{billNumber}", async (
                string billNumber,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(new GetBillDetailsByNumberQuery()
                {
                  BillNumber = billNumber,
                });
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetBillDetailsByNumber")
            .Produces<ApplicationResult<BillDetailDto>>(200)
            .Produces(400)
            .WithOpenApi(op => new(op)
            {
                Summary = "Get bill details by bill number",
                Description = "Returns BillDetailDto including items, payments, and refunds.",
                Tags = new List<OpenApiTag> { new() { Name = "Bills" } }
            });

        // GET /api/v1/bills/by-tracking/{trackingCode}?billType=WalletDeposit
        billGroup.MapGet("/by-tracking/{trackingCode}", async (
                string trackingCode,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(new GetBillDetailsByTrackingCodeQuery()
                {
                  
                  TrackingCode = trackingCode,
                });
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetBillDetailsByTrackingCode")
            .Produces<ApplicationResult<BillDetailDto>>(200)
            .Produces(400)
            .WithOpenApi(op => new(op)
            {
                Summary = "Get bill details by tracking code",
                Description = "Resolves a bill by tracking code (reference) and bill type; returns BillDetailDto.",
                Tags = new List<OpenApiTag> { new() { Name = "Bills" } }
            });

        // ---------- Resource actions ----------
        // POST /api/v1/bills/{billId}:issue
        billGroup.MapPost("/{billId:guid}/issue", async (
                Guid billId,
                [FromServices] IMediator mediator) =>
            {
                var command = new IssueBillCommand { BillId = billId };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("IssueBill")
            .Produces<ApplicationResult<IssueBillResponse>>(200)
            .Produces(400)
            .WithOpenApi(op => new(op)
            {
                Summary = "Issue a bill",
                Description = "Transitions a draft bill to the issued state.",
                Tags = new List<OpenApiTag> { new() { Name = "Bills" } }
            });

        // POST /api/v1/bills/{billId}:cancel
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
            .Produces<ApplicationResult<CancelBillResponse>>(200)
            .Produces(400)
            .WithOpenApi(op => new(op)
            {
                Summary = "Cancel a bill",
                Description = "Cancels an active bill; returns operation result.",
                Tags = new List<OpenApiTag> { new() { Name = "Bills" } }
            });

  
        
        return app;
    }
}

// ===== Requests =====
public sealed record CancelBillRequest
{
    public string? Reason { get; init; }
}
