using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Application.Queries.Wallets;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Finance.Presentation.Extensions;

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
        var walletGroup = app.MapGroup("/api/v1/me/wallets")
            .WithTags("Wallets")
            .RequireAuthorization();

        // ───────────────────── Create Wallet Deposit ─────────────────────
        walletGroup.MapPost("/deposit", async (
                [FromBody] CreateWalletDepositRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                // Get user information from current user service
                if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
                    return Results.Unauthorized();

                // Create command with user information
                var command = new CreateWalletDepositCommand
                {
                    ExternalUserId = currentUserService.UserId.Value,
                    UserFullName = currentUserService.UserFullName ?? string.Empty,
                    AmountRials = request.AmountRials,
                    Description = request.Description,
                    ExternalReference = request.ExternalReference,
                    Metadata = request.Metadata
                };

                var result = await mediator.Send(command);
                if (result.IsSuccess && result.Data != null)
                    return Results.Created($"/api/v1/me/wallets/deposits/{result.Data.DepositId}", result);
                return result.ToResult();
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
                if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
                    return Results.Unauthorized();
                
                var query = new GetUserWalletBalanceQuery { ExternalUserId = currentUserService.UserId.Value };
                var result = await mediator.Send(query);
                return result.ToResult();
            }) 
            .WithName("GetUserWalletBalance")
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
                if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
                    return Results.Unauthorized();
                
                var query = new ListUserWalletTransactionsQuery
                {
                    ExternalUserId = currentUserService.UserId.Value,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TransactionType = transactionType,
                    ReferenceId = referenceId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                var result = await mediator.Send(query);
                return result.ToResult();
            })
            .WithName("ListUserWalletTransactions")
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
                if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
                    return Results.Unauthorized();
                
                var query = new ListUserWalletDepositsQuery
                {
                    ExternalUserId = currentUserService.UserId.Value,
                    Page = page,
                    PageSize = pageSize,
                    Status = status,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                var result = await mediator.Send(query);
                return result.ToResult();
            })
            .WithName("ListUserWalletDeposits")
            .Produces<ApplicationResult<WalletDepositsResponse>>(200)
            .Produces(400);

        // ───────────────────── Get Wallet Deposit Details (by id) ─────────────────────
        walletGroup.MapGet("/deposits/{depositId:guid}", async (
                Guid depositId,
                [FromServices] ICurrentUserService currentUserService,
                [FromServices] IMediator mediator) =>
            {
                if (!currentUserService.IsAuthenticated || !currentUserService.UserId.HasValue)
                    return Results.Unauthorized();

                var query = new Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletDepositsDetails.GetWalletDepositsDetailsQuery
                {
                    DepositIds = new List<Guid> { depositId },
                    ExternalUserId = currentUserService.UserId.Value
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                    return result.ToResult();

                var item = result.Data?.FirstOrDefault();
                if (item == null)
                    return ResultsExtensions.FromApplicationResult(
                        ApplicationResult<Nezam.Refahi.Finance.Contracts.Dtos.WalletDepositDetailsDto>.NotFound("واریز مورد نظر یافت نشد."));

                return Results.Ok(ApplicationResult<Nezam.Refahi.Finance.Contracts.Dtos.WalletDepositDetailsDto>.Success(item));
            })
            .WithName("GetWalletDepositDetails")
            .Produces<ApplicationResult<Nezam.Refahi.Finance.Contracts.Dtos.WalletDepositDetailsDto>>(200)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(400);

        
        
        return app;
    }
}
