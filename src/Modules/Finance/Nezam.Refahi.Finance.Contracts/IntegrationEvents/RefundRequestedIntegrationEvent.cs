using MediatR;

namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a refund is requested
/// </summary>
public class RefundRequestedIntegrationEvent : INotification
{
    public Guid RefundId { get; set; }
    public Guid PaymentId { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public decimal RefundAmountRials { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string RequestedByNationalNumber { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
}