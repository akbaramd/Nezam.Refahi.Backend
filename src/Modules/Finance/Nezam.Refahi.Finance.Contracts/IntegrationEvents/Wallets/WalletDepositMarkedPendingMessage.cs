using MediatR;

namespace Nezam.Refahi.Contracts.Finance.v1.Messages;

/// <summary>
/// Published by orchestrator once the bill is created for the wallet deposit.
/// Indicates the deposit is Pending (awaiting payment).
/// </summary>
public class WalletDepositMarkedPendingMessage : INotification
{

    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    public Guid WalletDepositId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;

    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public decimal AmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = new();
}



