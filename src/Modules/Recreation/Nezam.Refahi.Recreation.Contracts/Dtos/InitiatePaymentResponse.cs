namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Response for InitiatePaymentCommand
/// </summary>
public class InitiatePaymentResponse
{
  public Guid BillId { get; set; } = Guid.Empty;
  public string BillNumber { get; set; } = string.Empty;
  public decimal TotalAmountRials { get; set; } = 0;
  public string PaymentUrl { get; set; } = string.Empty;
  public DateTime ExpiryDate { get; set; } = DateTime.MinValue;
}