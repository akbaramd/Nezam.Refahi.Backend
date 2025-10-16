using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Contracts;

namespace Nezam.Refahi.Recreation.Application.Dtos;

/// <summary>
/// Detailed tour reservation data transfer object
/// </summary>
public class ReservationDetailDto : IStaticMapper<TourReservation, ReservationDetailDto>
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public ReservationStatus Status { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public decimal? TotalAmountRials { get; set; }
    public string? Notes { get; set; }
    public Guid? CapacityId { get; set; }
    public Guid? BillId { get; set; }

    // Capacity information
    public TourCapacityDetailDto? Capacity { get; set; }

    // Tour information
    public TourSummaryDto Tour { get; set; } = null!;

    // Participants
    public List<ParticipantDto> Participants { get; set; } = new();

    // Summary information
    public int ParticipantCount { get; set; }
    public int MainParticipantCount { get; set; }
    public int GuestParticipantCount { get; set; }
    public bool IsExpired { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsPending { get; set; }

    // Additional domain behavior properties
    public bool IsActive { get; set; } // آیا رزرو فعال است؟
    public bool IsCancelled { get; set; } // آیا رزرو لغو شده است؟
    public bool IsTerminal { get; set; } // آیا رزرو در وضعیت نهایی است؟
    public bool IsDraft { get; set; } // آیا رزرو در وضعیت پیش‌نویس است؟
    public bool IsPaying { get; set; } // آیا رزرو در حال پرداخت است؟
    public bool IsSystemCancelled { get; set; } // آیا توسط سیستم لغو شده است؟

    // فیلدهای زمانی پیشرفته
    public TimeSpan? TimeUntilExpiry { get; set; } // زمان باقی‌مانده تا انقضا
    public TimeSpan? TimeUntilTourStart { get; set; } // زمان باقی‌مانده تا شروع تور
    public int DaysUntilTourStart { get; set; } // تعداد روزهای باقی‌مانده تا شروع تور
    public int HoursUntilTourStart { get; set; } // تعداد ساعت‌های باقی‌مانده تا شروع تور
    public bool IsExpiringSoon { get; set; } // آیا به زودی منقضی می‌شود؟
    public bool IsTourStartingSoon { get; set; } // آیا تور به زودی شروع می‌شود؟
    public string ExpiryStatusText { get; set; } = string.Empty; // متن وضعیت انقضا (فارسی)

    // فیلدهای وضعیت پیشرفته
    public string StatusText { get; set; } = string.Empty; // متن وضعیت (فارسی)
    public string StatusColor { get; set; } = string.Empty; // رنگ وضعیت
    public string StatusIcon { get; set; } = string.Empty; // آیکون وضعیت
    public bool CanBeCancelled { get; set; } // آیا قابل لغو است؟
    public bool CanBeModified { get; set; } // آیا قابل تغییر است؟
    public bool CanBeConfirmed { get; set; } // آیا قابل تأیید است؟
    public List<string> AvailableActions { get; set; } = new(); // اقدامات موجود
    public string NextActionText { get; set; } = string.Empty; // متن اقدام بعدی (فارسی)

    // فیلدهای پرداخت
    public bool HasPayment { get; set; } // آیا پرداخت دارد؟
    public bool IsPaymentPending { get; set; } // آیا پرداخت در انتظار است؟
    public bool IsPaymentCompleted { get; set; } // آیا پرداخت تکمیل شده است؟
    public string PaymentStatusText { get; set; } = string.Empty; // متن وضعیت پرداخت (فارسی)
    public DateTime? PaymentDate { get; set; } // تاریخ پرداخت
    public string PaymentMethod { get; set; } = string.Empty; // روش پرداخت
    public string PaymentReference { get; set; } = string.Empty; // مرجع پرداخت
    public bool CanRetryPayment { get; set; } // آیا می‌تواند پرداخت را دوباره امتحان کند؟

    // فیلدهای شرکت‌کنندگان
    public bool HasMainParticipant { get; set; } // آیا شرکت‌کننده اصلی دارد؟
    public bool HasGuestParticipants { get; set; } // آیا شرکت‌کنندگان مهمان دارد؟
    public int MaxAllowedGuests { get; set; } // حداکثر مهمان مجاز
    public bool CanAddMoreGuests { get; set; } // آیا می‌تواند مهمان بیشتری اضافه کند؟
    public int RemainingGuestSlots { get; set; } // تعداد جای خالی برای مهمان

    // فیلدهای تور
    public TourSummaryDto TourSummary { get; set; } = null!; // خلاصه تور
    public string TourTitle { get; set; } = string.Empty; // عنوان تور
    public DateTime TourStartDate { get; set; } // تاریخ شروع تور
    public DateTime TourEndDate { get; set; } // تاریخ پایان تور
    public string TourDuration { get; set; } = string.Empty; // مدت تور (فارسی)
    public string TourLocation { get; set; } = string.Empty; // مکان تور
    public bool IsTourActive { get; set; } // آیا تور فعال است؟
    public bool IsTourCancelled { get; set; } // آیا تور لغو شده است؟

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Maps from TourReservation entity to ReservationDetailDto
    /// Note: This is a basic mapping. Use the query handler for complete mapping with context
    /// </summary>
    /// <param name="entity">The source reservation entity</param>
    /// <returns>Mapped ReservationDetailDto</returns>
    public static ReservationDetailDto MapFrom(TourReservation entity)
    {
        return new ReservationDetailDto
        {
            Id = entity.Id,
            TourId = entity.TourId,
            TrackingCode = entity.TrackingCode,
            Status = entity.Status,
            ReservationDate = entity.ReservationDate,
            ExpiryDate = entity.ExpiryDate,
            ConfirmationDate = entity.ConfirmationDate,
            TotalAmountRials = entity.TotalAmount?.AmountRials,
            Notes = entity.Notes,
            CapacityId = entity.CapacityId,
            BillId = entity.BillId,
            Participants = entity.Participants.Select(ParticipantDto.MapFrom).ToList(),
            ParticipantCount = entity.GetParticipantCount(),
            MainParticipantCount = entity.Participants.Count(p => p.ParticipantType == ParticipantType.Member),
            GuestParticipantCount = entity.Participants.Count(p => p.ParticipantType == ParticipantType.Guest),
            IsExpired = entity.IsExpired(),
            IsConfirmed = entity.IsConfirmed(),
            IsPending = entity.IsPending(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.LastModifiedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedBy = entity.LastModifiedBy
        };
    }

    /// <summary>
    /// Maps from ReservationDetailDto to TourReservation entity
    /// Note: This operation is not supported as DTOs should not create entities
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>Not supported</returns>
    public static TourReservation MapTo(ReservationDetailDto dto)
    {
        throw new NotSupportedException("Mapping from ReservationDetailDto to TourReservation is not supported. Use domain services to create entities.");
    }
}

