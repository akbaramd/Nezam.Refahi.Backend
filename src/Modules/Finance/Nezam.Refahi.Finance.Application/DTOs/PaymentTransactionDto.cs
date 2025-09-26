namespace Nezam.Refahi.Finance.Application.DTOs;

/// <summary>
/// Payment transaction data transfer object
/// </summary>
public class PaymentTransactionDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public string Status { get; set; } = string.Empty; // PaymentStatus as string
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReference { get; set; }
    public string? GatewayResponse { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? ProcessedAmountRials { get; set; }
}
