using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nezam.Refahi.Finance.Contracts.Commands.Wallets;
using Nezam.Refahi.Finance.Contracts.Queries.Wallets;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Presentation.Endpoints;

/// <summary>
/// Request DTO for creating a wallet deposit
/// </summary>
public record CreateWalletDepositRequest
{
    /// <summary>
    /// Amount to deposit in rials
    /// </summary>
    public decimal AmountRials { get; init; }

    /// <summary>
    /// Description for the deposit
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// External reference (e.g., bank transaction ID)
    /// </summary>
    public string? ExternalReference { get; init; }

    /// <summary>
    /// Additional metadata for the deposit
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }
}

public static class WalletEndpoints
{
    public static WebApplication MapWalletEndpoints(this WebApplication app)
    {
        var walletGroup = app.MapGroup("/api/v1/wallets")
            .WithTags("Wallets")
            .RequireAuthorization();

        // ───────────────────── Create Wallet Deposit ─────────────────────
        walletGroup.MapPost("/deposit", async (
                [FromBody] CreateWalletDepositRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                // Get user information from current user service
                var externalUserId = currentUserService.UserId;
                if (externalUserId == null)
                {
                    return Results.Unauthorized();
                }

                // Create command with user information
                var command = new CreateWalletDepositCommand
                {
                    ExternalUserId = externalUserId.Value,
                    UserFullName = currentUserService.UserFullName ?? string.Empty,
                    AmountRials = request.AmountRials,
                    Description = request.Description,
                    ExternalReference = request.ExternalReference,
                    Metadata = request.Metadata
                };

                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Created($"/api/v1/wallets/deposits/{result.Data!.DepositId}", result) : Results.BadRequest(result);
            })
            .WithName("CreateWalletDeposit")
            .Produces<ApplicationResult<CreateWalletDepositResponse>>(201)
            .Produces(400)
            .Produces(401);


        // ───────────────────── Get Wallet Balance ─────────────────────
        walletGroup.MapGet("/balance", async (
                [FromServices] ICurrentUserService currentUserService,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetWalletBalanceQuery { ExternalUserId = currentUserService.UserId!.Value };
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
                    ExternalUserId = currentUserService.UserId!.Value,
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
                    ExternalUserId = currentUserService.UserId!.Value,
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
