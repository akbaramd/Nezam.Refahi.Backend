using MediatR;

namespace Nezam.Refahi.Contracts.Finance.v1.Messages;

public class WalletDepositCompletedEventMessage : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    public required Guid WalletDepositId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;

    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    public decimal AmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public Guid PaymentId { get; set; }

    public DateTime CompletedAt { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}


