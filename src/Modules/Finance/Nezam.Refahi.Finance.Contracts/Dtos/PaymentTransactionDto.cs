using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Contracts.Dtos;

/// <summary>
/// Payment transaction data transfer object
/// </summary>
public class PaymentTransactionDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public PaymentStatus Status { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReference { get; set; }
    public string? GatewayResponse { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? ProcessedAmountRials { get; set; }
}