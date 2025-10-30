namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Participant data transfer object
/// </summary>
public class ParticipantDto
{
  public Guid Id { get; set; }
  public Guid ReservationId { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty;
  public string NationalNumber { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string? Email { get; set; }
  public string ParticipantType { get; set; } = string.Empty;
  public DateTime? BirthDate { get; set; }
  public string? EmergencyContactName { get; set; }
  public string? EmergencyContactPhone { get; set; }
  public string? Notes { get; set; }
  public decimal RequiredAmountRials { get; set; }
  public decimal? PaidAmountRials { get; set; }
  public DateTime? PaymentDate { get; set; }
  public DateTime RegistrationDate { get; set; }

  // Calculated properties
  public bool HasPaid { get; set; }
  public bool IsFullyPaid { get; set; }
  public decimal RemainingAmountRials { get; set; }
  public bool IsMainParticipant { get; set; }
  public bool IsGuest { get; set; }

  // Domain behavior properties
  public bool IsPaymentPending { get; set; } // آیا پرداخت در انتظار است؟
  public bool IsPaymentOverdue { get; set; } // آیا پرداخت معوق است؟
  public bool CanMakePayment { get; set; } // آیا می‌تواند پرداخت کند؟
  public bool IsPaymentRequired { get; set; } // آیا پرداخت الزامی است؟

  // فیلدهای اطلاعاتی پیشرفته
  public string ParticipantTypeText { get; set; } = string.Empty; // متن نوع شرکت‌کننده (فارسی)
  public string AgeGroup { get; set; } = string.Empty; // گروه سنی
  public int Age { get; set; } // سن
  public bool IsEligible { get; set; } // آیا واجد شرایط است؟
  public List<string> EligibilityIssues { get; set; } = new(); // مسائل واجد شرایط بودن
  public string ContactPhone { get; set; } = string.Empty; // شماره تماس
  public string ContactEmail { get; set; } = string.Empty; // ایمیل تماس

  // فیلدهای قیمت‌گذاری پیشرفته
  public decimal BasePriceRials { get; set; } // قیمت پایه (ریال)
  public decimal? DiscountPercentage { get; set; } // درصد تخفیف
  public decimal EffectivePriceRials { get; set; } // قیمت مؤثر (ریال)
  public decimal? DiscountAmountRials { get; set; } // مبلغ تخفیف (ریال)
  public string PriceNote { get; set; } = string.Empty; // یادداشت قیمت
  public bool HasDiscount { get; set; } // آیا تخفیف دارد؟
  public string DiscountReason { get; set; } = string.Empty; // دلیل تخفیف
}