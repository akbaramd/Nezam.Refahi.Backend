using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Wallets;

/// <summary>
/// Command to charge a wallet with a specific amount
/// </summary>
public record ChargeWalletCommand : IRequest<ApplicationResult<ChargeWalletResponse>>
{
    /// <summary>
    /// User's national number
    /// </summary>
    public string UserNationalNumber { get; init; } = string.Empty;

    /// <summary>
    /// Amount to charge in rials
    /// </summary>
    public decimal AmountRials { get; init; }

    /// <summary>
    /// Reference ID for tracking the charge operation
    /// </summary>
    public string ReferenceId { get; init; } = string.Empty;

    /// <summary>
    /// External reference (e.g., bank transaction ID)
    /// </summary>
    public string? ExternalReference { get; init; }

    /// <summary>
    /// Description of the charge operation
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Additional metadata for the charge
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Response for ChargeWalletCommand
/// </summary>
public record ChargeWalletResponse
{
    public Guid WalletId { get; init; }
    public Guid TransactionId { get; init; }
    public string UserNationalNumber { get; init; } = string.Empty;
    public decimal AmountRials { get; init; }
    public decimal PreviousBalanceRials { get; init; }
    public decimal NewBalanceRials { get; init; }
    public string TransactionType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; }
    public string ReferenceId { get; init; } = string.Empty;
    public string? ExternalReference { get; init; }
}
