using MediatR;

namespace Nezam.Refahi.Contracts.Finance.v1.Messages;

public class WalletDepositPendingFailedEventMessage : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    public Guid WalletDepositId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;


    public string FailureReason { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}



