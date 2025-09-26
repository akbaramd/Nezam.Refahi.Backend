using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Wallets;

/// <summary>
/// Command to create a new wallet for a user
/// </summary>
public record CreateWalletCommand : IRequest<ApplicationResult<CreateWalletResponse>>
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
    /// Initial balance in rials (optional, defaults to 0)
    /// </summary>
    public decimal InitialBalanceRials { get; init; } = 0;

    /// <summary>
    /// Description for the wallet creation
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Additional metadata for the wallet
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Response for CreateWalletCommand
/// </summary>
public record CreateWalletResponse
{
    public Guid WalletId { get; init; }
    public Guid UserExternalUserId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public decimal InitialBalanceRials { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
