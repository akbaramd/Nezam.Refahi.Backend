using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;
using Nezam.Refahi.Finance.Contracts.Commands.Wallets;
using Nezam.Refahi.Finance.Contracts.Queries.Wallets;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Presentation.Endpoints;

public static class WalletEndpoints
{
    public static WebApplication MapWalletEndpoints(this WebApplication app)
    {
        var walletGroup = app.MapGroup("/api/v1/wallets")
            .WithTags("Wallets")
            .RequireAuthorization();

        // ───────────────────── Create Wallet Deposit Bill ─────────────────────
        walletGroup.MapPost("/deposit-bill", async (
                [FromBody] CreateBillChargeBillCommand command,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Created($"/api/v1/bills/{result.Data!.BillId}", result) : Results.BadRequest(result);
            })
            .WithName("CreateWalletDepositBill")
            .Produces<ApplicationResult<CreateBillChargeBillResponse>>(201)
            .Produces(400);


        // ───────────────────── Get Wallet Balance ─────────────────────
        walletGroup.MapGet("/balance", async (
                [FromServices] ICurrentUserService currentUserService,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetWalletBalanceQuery { UserNationalNumber = currentUserService.UserNationalNumber!.ToString() };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetWalletBalance")
            .Produces<ApplicationResult<WalletBalanceResponse>>(200)
            .Produces(400);

        // ───────────────────── Get Wallet Transactions ─────────────────────
        walletGroup.MapGet("/transactions", async (
                [FromServices] ICurrentUserService currentUserService,
                [FromServices] IMediator mediator,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 20,
                [FromQuery] string? transactionType = null,
                [FromQuery] string? referenceId = null,
                [FromQuery] DateTime? fromDate = null,
                [FromQuery] DateTime? toDate = null) =>
            {
                var query = new GetWalletTransactionsQuery
                {
                    UserNationalNumber = currentUserService.UserNationalNumber!.ToString(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TransactionType = transactionType,
                    ReferenceId = referenceId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetWalletTransactions")
            .Produces<ApplicationResult<WalletTransactionsResponse>>(200)
            .Produces(400);

        // ───────────────────── Get Wallet Deposits ─────────────────────
        walletGroup.MapGet("/deposits", async (
                [FromServices] ICurrentUserService currentUserService,
                [FromServices] IMediator mediator,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20,
                [FromQuery] string? status = null,
                [FromQuery] DateTime? fromDate = null,
                [FromQuery] DateTime? toDate = null) =>
            {
                var query = new GetWalletDepositsQuery
                {
                    UserNationalNumber = currentUserService.UserNationalNumber!.ToString(),
                    Page = page,
                    PageSize = pageSize,
                    Status = status,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetWalletDeposits")
            .Produces<ApplicationResult<WalletDepositsResponse>>(200)
            .Produces(400);

        return app;
    }
}
