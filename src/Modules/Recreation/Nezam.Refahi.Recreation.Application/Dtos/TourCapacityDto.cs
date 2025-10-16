using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Application.Dtos;

/// <summary>
/// Tour capacity data transfer object with detailed capacity information
/// </summary>
public class TourCapacityDto
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public int MaxParticipants { get; set; }
    public int RemainingParticipants { get; set; }
    public int AllocatedParticipants { get; set; }
    public double UtilizationPercentage { get; set; }
    public int MinParticipantsPerReservation { get; set; }
    public int MaxParticipantsPerReservation { get; set; }
    
    public DateTime RegistrationStart { get; set; }
    public DateTime RegistrationEnd { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public bool IsEffectiveFor { get; set; }
    public bool IsFullyBooked { get; set; }
    public bool IsNearlyFull { get; set; } // When > 80% booked

    // Domain behavior properties
    public bool IsExpired { get; set; } // آیا ظرفیت منقضی شده است؟
    public bool IsAvailable { get; set; } // آیا ظرفیت در دسترس است؟
    public bool IsClosed { get; set; } // آیا ظرفیت بسته است؟
    public bool CanAcceptReservations { get; set; } // آیا می‌تواند رزرو بپذیرد؟
    
    // Capacity state enum
    public CapacityState CapacityState { get; set; }
    
    // Availability status for UI
    public string AvailabilityStatus { get; set; } = string.Empty; // Available, Nearly Full, Full, Closed
    public string AvailabilityMessage { get; set; } = string.Empty; // Human readable message

    // فیلدهای ظرفیت پیشرفته
    public int ConfirmedParticipants { get; set; } // تعداد شرکت‌کنندگان تأیید شده
    public int PendingParticipants { get; set; } // تعداد شرکت‌کنندگان در انتظار
    public int PayingParticipants { get; set; } // تعداد شرکت‌کنندگان در حال پرداخت
    public int ExpiredParticipants { get; set; } // تعداد شرکت‌کنندگان منقضی شده
    public string CapacityStatusText { get; set; } = string.Empty; // متن وضعیت ظرفیت (فارسی)
    public string CapacityMessage { get; set; } = string.Empty; // پیام ظرفیت (فارسی)
    public bool IsCapacityCritical { get; set; } // آیا ظرفیت در وضعیت بحرانی است؟
    public int AvailableSpots { get; set; } // تعداد جای خالی

    // فیلدهای زمانی
    public TimeSpan? TimeUntilRegistrationEnd { get; set; } // زمان باقی‌مانده تا پایان ثبت‌نام
    public int DaysUntilRegistrationEnd { get; set; } // تعداد روزهای باقی‌مانده تا پایان ثبت‌نام
    public int HoursUntilRegistrationEnd { get; set; } // تعداد ساعت‌های باقی‌مانده تا پایان ثبت‌نام
    public bool IsRegistrationEndingSoon { get; set; } // آیا ثبت‌نام به زودی تمام می‌شود؟
    public string RegistrationStatusText { get; set; } = string.Empty; // متن وضعیت ثبت‌نام (فارسی)
    public bool IsRegistrationClosed { get; set; } // آیا ثبت‌نام بسته است؟
    public bool IsRegistrationNotStarted { get; set; } // آیا ثبت‌نام شروع نشده است؟
}