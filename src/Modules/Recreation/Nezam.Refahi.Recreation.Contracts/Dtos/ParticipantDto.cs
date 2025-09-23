using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Contracts;

namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Participant data transfer object
/// </summary>
public class ParticipantDto : IStaticMapper<Participant, ParticipantDto>
{
  public Guid Id { get; set; }
  public Guid ReservationId { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty;
  public string NationalNumber { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty;
  public string? Email { get; set; }
  public ParticipantType ParticipantType { get; set; }
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

  /// <summary>
  /// Maps from Participant entity to ParticipantDto
  /// </summary>
  /// <param name="entity">The source participant entity</param>
  /// <returns>Mapped ParticipantDto</returns>
  public static ParticipantDto MapFrom(Participant entity)
  {
    return new ParticipantDto
    {
      Id = entity.Id,
      ReservationId = entity.ReservationId,
      FirstName = entity.FirstName,
      LastName = entity.LastName,
      FullName = entity.FullName,
      NationalNumber = entity.NationalNumber,
      PhoneNumber = entity.PhoneNumber,
      Email = entity.Email,
      ParticipantType = entity.ParticipantType,
      BirthDate = entity.BirthDate,
      EmergencyContactName = entity.EmergencyContactName,
      EmergencyContactPhone = entity.EmergencyContactPhone,
      Notes = entity.Notes,
      RequiredAmountRials = entity.RequiredAmount.AmountRials,
      PaidAmountRials = entity.PaidAmount?.AmountRials,
      PaymentDate = entity.PaymentDate,
      RegistrationDate = entity.RegistrationDate,
      HasPaid = entity.HasPaid,
      IsFullyPaid = entity.IsFullyPaid,
      RemainingAmountRials = entity.RemainingAmount.AmountRials,
      IsMainParticipant = entity.IsMainParticipant,
      IsGuest = entity.IsGuest
    };
  }

  /// <summary>
  /// Maps from ParticipantDto to Participant entity
  /// Note: This operation is not supported as DTOs should not create entities
  /// </summary>
  /// <param name="dto">The DTO to map from</param>
  /// <returns>Not supported</returns>
  public static Participant MapTo(ParticipantDto dto)
  {
    throw new NotSupportedException("Mapping from ParticipantDto to Participant is not supported. Use domain services to create entities.");
  }
}