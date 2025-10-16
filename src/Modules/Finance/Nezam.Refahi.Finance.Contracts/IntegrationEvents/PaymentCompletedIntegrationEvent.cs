using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a payment is completed successfully
/// </summary>
public class PaymentCompletedIntegrationEvent : INotification
{
    public Guid PaymentId { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public decimal AmountRials { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReference { get; set; }
    public DateTime CompletedAt { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}