namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Response for InitiatePaymentCommand
/// </summary>
public class InitiatePaymentResponse
{
  public Guid BillId { get; init; }
  public string BillNumber { get; init; } = string.Empty;
  public decimal TotalAmountRials { get; init; }
  public string PaymentUrl { get; init; } = string.Empty;
  public DateTime ExpiryDate { get; init; }
}