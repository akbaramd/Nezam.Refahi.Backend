using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a refund is completed successfully
/// </summary>
public class RefundCompletedIntegrationEvent : INotification
{
    public Guid RefundId { get; set; }
    public Guid PaymentId { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public decimal RefundAmountRials { get; set; }
    public string RequestedByNationalNumber { get; set; } = string.Empty;
    public string? GatewayRefundId { get; set; }
    public DateTime CompletedAt { get; set; }
}