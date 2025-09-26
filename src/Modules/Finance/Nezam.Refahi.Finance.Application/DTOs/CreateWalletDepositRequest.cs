namespace Nezam.Refahi.Finance.Application.DTOs;

/// <summary>
/// Request DTO for creating a wallet deposit
/// </summary>
public record CreateWalletDepositRequest
{
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
