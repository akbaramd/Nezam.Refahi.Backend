namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a payment fails
/// </summary>
public class PaymentFailedIntegrationEvent
{
    public Guid PaymentId { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public string? FailureReason { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime FailedAt { get; set; } = DateTime.UtcNow;
}