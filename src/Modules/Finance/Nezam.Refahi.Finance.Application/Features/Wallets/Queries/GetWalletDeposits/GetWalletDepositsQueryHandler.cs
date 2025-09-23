using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Contracts.Queries.Wallets;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletDeposits;

/// <summary>
/// Handler for GetWalletDepositsQuery
/// </summary>
public class GetWalletDepositsQueryHandler : IRequestHandler<GetWalletDepositsQuery, ApplicationResult<WalletDepositsResponse>>
{
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly IValidator<GetWalletDepositsQuery> _validator;

    public GetWalletDepositsQueryHandler(
        IWalletDepositRepository walletDepositRepository,
        IValidator<GetWalletDepositsQuery> validator)
    {
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ApplicationResult<WalletDepositsResponse>> Handle(
        GetWalletDepositsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate the query
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ApplicationResult<WalletDepositsResponse>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            // Build query
            var query = _walletDepositRepository.GetAll()
                .Where(d => d.UserNationalNumber == request.UserNationalNumber);

            // Apply filters
            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<WalletDepositStatus>(request.Status, true, out var status))
                {
                    query = query.Where(d => d.Status == status);
                }
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(d => d.RequestedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(d => d.RequestedAt <= request.ToDate.Value);
            }

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Calculate pagination
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;

            // Get deposits with pagination
            var deposits = await query
                .OrderByDescending(d => d.RequestedAt)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(d => new WalletDepositDto
                {
                    DepositId = d.Id,
                    WalletId = d.WalletId,
                    BillId = d.BillId,
                    UserNationalNumber = d.UserNationalNumber,
                    AmountRials = d.Amount.AmountRials,
                    Status = d.Status.ToString(),
                    Description = d.Description,
                    ExternalReference = d.ExternalReference,
                    RequestedAt = d.RequestedAt,
                    CompletedAt = d.CompletedAt,
                    BillNumber = d.Bill != null ? d.Bill.BillNumber : null,
                    Metadata = d.Metadata
                })
                .ToListAsync(cancellationToken);

            // Build response
            var response = new WalletDepositsResponse
            {
                UserNationalNumber = request.UserNationalNumber,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                Deposits = deposits
            };

            return ApplicationResult<WalletDepositsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<WalletDepositsResponse>.Failure(
                $"An error occurred while retrieving wallet deposits: {ex.Message}");
        }
    }
}
