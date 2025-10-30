using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Wallets;

public record MarkWalletDepositPendingCommand : IRequest<ApplicationResult<Unit>>
{
    public Guid WalletDepositId { get; init; }
}


