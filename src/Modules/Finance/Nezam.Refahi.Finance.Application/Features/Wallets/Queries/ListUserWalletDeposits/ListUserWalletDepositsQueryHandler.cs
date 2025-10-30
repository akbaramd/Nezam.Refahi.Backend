using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Application.Queries.Wallets;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletDeposits;

/// <summary>
/// Handler for ListUserWalletDepositsQuery
/// </summary>
public class GetWalletDepositsQueryHandler : IRequestHandler<ListUserWalletDepositsQuery, ApplicationResult<WalletDepositsResponse>>
{
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly IValidator<ListUserWalletDepositsQuery> _validator;

    public GetWalletDepositsQueryHandler(
        IWalletDepositRepository walletDepositRepository,
        IValidator<ListUserWalletDepositsQuery> validator)
    {
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ApplicationResult<WalletDepositsResponse>> Handle(
        ListUserWalletDepositsQuery request,
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

            // Get deposits by user national number
            var allDeposits = await _walletDepositRepository.GetByExternalUserIdAsync(request.ExternalUserId, cancellationToken);

            // Apply filters
            var filteredDeposits = allDeposits.AsQueryable();

            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<WalletDepositStatus>(request.Status, true, out var status))
                {
                    filteredDeposits = filteredDeposits.Where(d => d.Status == status);
                }
            }

            if (request.FromDate.HasValue)
            {
                filteredDeposits = filteredDeposits.Where(d => d.RequestedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                filteredDeposits = filteredDeposits.Where(d => d.RequestedAt <= request.ToDate.Value);
            }

            // Get total count
            var totalCount = filteredDeposits.Count();

            // Calculate pagination
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;

            // Get deposits with pagination
            var deposits = filteredDeposits
                .OrderByDescending(d => d.RequestedAt)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(d => new WalletDepositDto
                {
                    DepositId = d.Id,
                    WalletId = d.WalletId,
                    TrackingCode = d.TrackingCode,
                    ExternalUserId = d.ExternalUserId,
                    AmountRials = (long)d.Amount.AmountRials,
                    Status = d.Status.ToString(),
                    StatusText = GetWalletDepositStatusText(d.Status),
                    Description = d.Description,
                    ExternalReference = d.ExternalReference,
                    RequestedAt = d.RequestedAt,
                    CompletedAt = d.CompletedAt,
                    Metadata = d.Metadata
                })
                .ToList();

            // Build response
            var response = new WalletDepositsResponse
            {
                ExternalUserId = request.ExternalUserId,
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

    /// <summary>
    /// Convert wallet deposit status to Persian text
    /// </summary>
    private static string GetWalletDepositStatusText(WalletDepositStatus status)
    {
        return status switch
        {
            WalletDepositStatus.Pending => "در انتظار",
            WalletDepositStatus.Completed => "تکمیل شده",
            WalletDepositStatus.Failed => "ناموفق",
            WalletDepositStatus.Cancelled => "لغو شده",
            _ => status.ToString()
        };
    }
}
