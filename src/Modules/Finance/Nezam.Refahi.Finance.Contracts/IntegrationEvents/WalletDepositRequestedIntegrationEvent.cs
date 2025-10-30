using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Raised when a user requests to deposit money into their wallet.
/// Orchestrator listens and creates a bill for the deposit.
/// </summary>
public class WalletDepositRequestedIntegrationEvent : INotification
{
    public Guid WalletDepositId { get; set; } = Guid.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public decimal AmountRials { get; set; }
    public string Currency { get; set; } = "IRR";
    public string? Description { get; set; }



    public Dictionary<string, string> Metadata { get; set; } = new();
}


