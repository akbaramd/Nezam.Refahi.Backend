using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Wallets;

public record HandleFailedWalletDepositCommand : IRequest<ApplicationResult<Unit>>
{
    public Guid WalletDepositId { get; init; }
    public string FailureStage { get; init; } = string.Empty;
    public string FailureReason { get; init; } = string.Empty;
    public string? ErrorCode { get; init; }
    public Guid? BillId { get; init; }
    public string? BillNumber { get; init; }
    public Guid? PaymentId { get; init; }
}


