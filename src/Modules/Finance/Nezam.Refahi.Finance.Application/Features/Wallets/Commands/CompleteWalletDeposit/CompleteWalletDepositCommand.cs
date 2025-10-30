using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Wallets;

public record CompleteWalletDepositCommand : IRequest<ApplicationResult<Unit>>
{
    public Guid WalletDepositId { get; init; }
    public Guid ExternalUserId { get; init; }
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public Guid PaymentId { get; init; }
    public DateTime PaidAt { get; init; }
    public string TrackingCode { get; init; } = string.Empty;
}


