using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Wallets;

/// <summary>
/// Command to create a wallet deposit with bill generation
/// </summary>
public record CreateWalletDepositCommand : IRequest<ApplicationResult<CreateWalletDepositResponse>>
{
    /// <summary>
    /// User's national number
    /// </summary>
    public Guid ExternalUserId { get; init; }

    /// <summary>
    /// User's full name
    /// </summary>
    public string UserFullName { get; init; } = string.Empty;

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

/// <summary>
/// Response for CreateWalletDepositCommand
/// </summary>
public record CreateWalletDepositResponse
{
    public Guid DepositId { get; init; }
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public Guid UserExternalUserId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public decimal AmountRials { get; init; }
    public string DepositStatus { get; init; } = string.Empty;
    public string DepositStatusText { get; init; } = string.Empty;  // Persian status text
    public string BillStatus { get; init; } = string.Empty;
    public string BillStatusText { get; init; } = string.Empty;  // Persian status text
    public DateTime RequestedAt { get; init; }
    public DateTime? BillIssueDate { get; init; }
    public string ReferenceId { get; init; } = string.Empty;
}
