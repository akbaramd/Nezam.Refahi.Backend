namespace Nezam.Refahi.Finance.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a reservation payment is requested
/// </summary>
public class ReservationPaymentRequestedEvent
{
    public Guid ReservationId { get; set; }
    public string UserNationalNumber { get; set; } = string.Empty;
    public long AmountRials { get; set; }
    public string CallbackUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}