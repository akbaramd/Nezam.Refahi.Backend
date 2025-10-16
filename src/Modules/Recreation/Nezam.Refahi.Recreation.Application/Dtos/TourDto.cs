using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Application.Dtos;

/// <summary>
/// Tour data transfer object containing all tour information
/// </summary>
public class TourDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime TourStart { get; set; }
    public DateTime TourEnd { get; set; }

    // Capacity information (calculated from active capacities)
    public int MaxCapacity { get; set; }
    public int RemainingCapacity { get; set; }
    public int ReservedCapacity { get; set; }
    public double CapacityUtilizationPercentage { get; set; }
    public bool IsFullyBooked { get; set; }
    public bool IsNearlyFull { get; set; } // When > 80% booked


    public bool IsActive { get; set; }

    // Tour status and capacity state
    public TourStatus Status { get; set; }
    public CapacityState CapacityState { get; set; }

    // Age restrictions
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }

    // Guest limitations per reservation
    public int? MaxGuestsPerReservation { get; set; }

    // Registration period (calculated from active capacities)
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public bool IsRegistrationOpen { get; set; }

    // Tour capacities collection
    public List<TourCapacityDto> Capacities { get; set; } = new();

    // Member requirements
    public List<string> RequiredCapabilities { get; set; } = new();
    public List<string> RequiredFeatures { get; set; } = new();

    // Tour features
    public List<TourFeatureDto> Features { get; set; } = new();

    // Restricted tours
    public List<RestrictedTourDto> RestrictedTours { get; set; } = new();

    // Pricing information
    public List<TourPricingDto> Pricing { get; set; } = new();

    // Photos
    public List<TourPhotoDto> Photos { get; set; } = new();

    // Participants information (summary)
    public int TotalParticipants { get; set; }
    public int MainParticipants { get; set; }
    public int GuestParticipants { get; set; }

    // User reservation status and ID
    public ReservationStatus? UserReservationStatus { get; set; }
    public Guid? UserReservationId { get; set; }

    // فیلدهای زمانی و وضعیت
    public TimeSpan? TimeUntilTourStart { get; set; } // زمان باقی‌مانده تا شروع تور
    public TimeSpan? TimeUntilRegistrationEnd { get; set; } // زمان باقی‌مانده تا پایان ثبت‌نام
    public string TourStatusText { get; set; } = string.Empty; // متن وضعیت تور (فارسی)
    public string RegistrationStatusText { get; set; } = string.Empty; // متن وضعیت ثبت‌نام (فارسی)
    public bool IsRegistrationEndingSoon { get; set; } // آیا ثبت‌نام به زودی تمام می‌شود؟
    public bool IsTourStartingSoon { get; set; } // آیا تور به زودی شروع می‌شود؟
    public int DaysUntilTourStart { get; set; } // تعداد روزهای باقی‌مانده تا شروع تور
    public int HoursUntilTourStart { get; set; } // تعداد ساعت‌های باقی‌مانده تا شروع تور

    // فیلدهای ظرفیت پیشرفته
    public int ConfirmedParticipants { get; set; } // تعداد شرکت‌کنندگان تأیید شده
    public int PendingParticipants { get; set; } // تعداد شرکت‌کنندگان در انتظار
    public int PayingParticipants { get; set; } // تعداد شرکت‌کنندگان در حال پرداخت
    public int ExpiredParticipants { get; set; } // تعداد شرکت‌کنندگان منقضی شده
    public string CapacityStatusText { get; set; } = string.Empty; // متن وضعیت ظرفیت (فارسی)
    public string CapacityMessage { get; set; } = string.Empty; // پیام ظرفیت (فارسی)
    public bool IsCapacityCritical { get; set; } // آیا ظرفیت در وضعیت بحرانی است؟
    public int AvailableSpotsForUser { get; set; } // تعداد جای خالی برای کاربر فعلی

    // فیلدهای قیمت‌گذاری
    public decimal? LowestPriceRials { get; set; } // کمترین قیمت (ریال)
    public decimal? HighestPriceRials { get; set; } // بالاترین قیمت (ریال)
    public decimal? AveragePriceRials { get; set; } // میانگین قیمت (ریال)
    public bool HasDiscount { get; set; } // آیا تخفیف دارد؟
    public decimal? MaxDiscountPercentage { get; set; } // حداکثر درصد تخفیف
    public string PriceRangeText { get; set; } = string.Empty; // متن محدوده قیمت (فارسی)
    public bool IsEarlyBirdAvailable { get; set; } // آیا پیش‌خرید زودهنگام موجود است؟
    public bool IsLastMinuteAvailable { get; set; } // آیا آخرین لحظه موجود است؟

    // فیلدهای کاربری
    public bool CanUserReserve { get; set; } // آیا کاربر می‌تواند رزرو کند؟
    public List<string> ReservationBlockReasons { get; set; } = new(); // دلایل عدم امکان رزرو
    public bool HasUserReservation { get; set; } // آیا کاربر رزرو دارد؟
    public bool CanUserCancel { get; set; } // آیا کاربر می‌تواند لغو کند؟
    public bool CanUserModify { get; set; } // آیا کاربر می‌تواند تغییر دهد؟
    public string UserReservationStatusText { get; set; } = string.Empty; // متن وضعیت رزرو کاربر (فارسی)
    public DateTime? UserReservationExpiryDate { get; set; } // تاریخ انقضای رزرو کاربر
    public TimeSpan? TimeUntilUserReservationExpiry { get; set; } // زمان باقی‌مانده تا انقضای رزرو کاربر

    // Domain behavior properties for user reservation
    public bool IsUserReservationExpired { get; set; } // آیا رزرو کاربر منقضی شده است؟
    public bool IsUserReservationActive { get; set; } // آیا رزرو کاربر فعال است؟
    public bool IsUserReservationPending { get; set; } // آیا رزرو کاربر در انتظار است؟
    public bool IsUserReservationConfirmed { get; set; } // آیا رزرو کاربر تأیید شده است؟
    public bool IsUserReservationCancelled { get; set; } // آیا رزرو کاربر لغو شده است？
    public bool IsUserReservationTerminal { get; set; } // آیا رزرو کاربر در وضعیت نهایی است؟

    // فیلدهای آماری و تحلیلی
    public int TotalReservationsCount { get; set; } // تعداد کل رزروها
    public int ConfirmedReservationsCount { get; set; } // تعداد رزروهای تأیید شده
    public int PendingReservationsCount { get; set; } // تعداد رزروهای در انتظار
    public int CancelledReservationsCount { get; set; } // تعداد رزروهای لغو شده
    public double ReservationSuccessRate { get; set; } // نرخ موفقیت رزروها
    public int ViewCount { get; set; } // تعداد بازدید
    public DateTime? LastViewedAt { get; set; } // آخرین بازدید
    public bool IsPopular { get; set; } // آیا محبوب است؟
    public bool IsTrending { get; set; } // آیا در حال ترند است؟

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}