using MediatR;
using Nezam.Refahi.Finance.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletDepositsDetails;

public sealed class GetWalletDepositsDetailsQuery : IRequest<ApplicationResult<List<WalletDepositDetailsDto>>>
{
    public List<Guid> DepositIds { get; init; } = new();
    public Guid ExternalUserId { get; init; }
}

