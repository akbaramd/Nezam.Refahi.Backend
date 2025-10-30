using MediatR;
using Nezam.Refahi.Finance.Contracts.Dtos;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletDepositsDetails;

public sealed class GetWalletDepositsDetailsQueryHandler : IRequestHandler<GetWalletDepositsDetailsQuery, ApplicationResult<List<WalletDepositDetailsDto>>>
{
    private readonly IWalletDepositRepository _walletDepositRepository;

    public GetWalletDepositsDetailsQueryHandler(IWalletDepositRepository walletDepositRepository)
    {
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
    }

    public async Task<ApplicationResult<List<WalletDepositDetailsDto>>> Handle(GetWalletDepositsDetailsQuery request, CancellationToken cancellationToken)
    {
        if (request.DepositIds == null || request.DepositIds.Count == 0)
            return ApplicationResult<List<WalletDepositDetailsDto>>.Success(new List<WalletDepositDetailsDto>());

        var results = new List<WalletDepositDetailsDto>(request.DepositIds.Count);

        foreach (var depositId in request.DepositIds)
        {
            var deposit = await _walletDepositRepository.FindOneAsync(d => d.Id == depositId, cancellationToken);
            if (deposit == null)
                continue;

            // Authorization check: only allow the owner to view
            if (deposit.ExternalUserId != request.ExternalUserId)
            {
                return ApplicationResult<List<WalletDepositDetailsDto>>.Failure(
                    new List<string> { "دسترسی مجاز نیست" },
                    "FORBIDDEN");
            }

            results.Add(new WalletDepositDetailsDto
            {
                DepositId = deposit.Id,
                WalletId = deposit.WalletId,
                ExternalUserId = deposit.ExternalUserId,
                TrackingCode = deposit.TrackingCode,
                AmountRials = deposit.Amount.AmountRials,
                Currency = deposit.Amount.Currency,
                Status = deposit.Status.ToString(),
                RequestedAt = deposit.RequestedAt,
                CompletedAt = deposit.CompletedAt
            });
        }

        return ApplicationResult<List<WalletDepositDetailsDto>>.Success(results);
    }
}


