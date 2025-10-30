using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

public class WalletChargeCompletedIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty; // deposit tracking code
    public string ReferenceType { get; set; } = string.Empty; // WalletDeposit
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public decimal AmountRials { get; set; }
    public DateTime ChargedAt { get; set; }
    public Guid WalletTransactionId { get; set; }
    public decimal NewWalletBalance { get; set; }
    public Guid PaymentId { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReference { get; set; }
    public string Gateway { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}


