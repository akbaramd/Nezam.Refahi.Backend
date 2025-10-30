using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Published when a wallet deposit is completed (bill fully paid).
/// </summary>
public class WalletDepositCompletedIntegrationEvent : INotification
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
    public Guid PaymentId { get; set; }
    public DateTime PaidAt { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = new();
}


