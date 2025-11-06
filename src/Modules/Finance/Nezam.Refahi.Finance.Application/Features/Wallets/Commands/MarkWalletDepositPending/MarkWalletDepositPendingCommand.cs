using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Wallets;

public record MarkWalletDepositAwaitingPaymentCommand : IRequest<ApplicationResult<Unit>>
{
    public Guid WalletDepositId { get; init; }
}


