namespace Nezam.Refahi.Finance.Contracts.Dtos;

public sealed class WalletDepositDetailsDto
{
    public Guid DepositId { get; init; }
    public Guid WalletId { get; init; }
    public Guid ExternalUserId { get; init; }
    public string TrackingCode { get; init; } = string.Empty;
    public decimal AmountRials { get; init; }
    public string Currency { get; init; } = "IRR";
    public string Status { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}


