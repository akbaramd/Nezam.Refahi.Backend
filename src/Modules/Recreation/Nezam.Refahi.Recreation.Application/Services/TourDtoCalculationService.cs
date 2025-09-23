using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Services;

/// <summary>
/// سرویس محاسبه فیلدهای پیشرفته برای DTOهای تور
/// </summary>
public class TourDtoCalculationService
{
    private readonly ITourReservationRepository _reservationRepository;

    public TourDtoCalculationService(ITourReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    /// <summary>
    /// محاسبه فیلدهای زمانی و وضعیت برای TourDto
    /// </summary>
    public Task CalculateTimeAndStatusFieldsAsync(TourDto dto, Tour tour, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;

        // محاسبه فیلدهای زمانی
        dto.TimeUntilTourStart = tour.TourStart - currentTime;
        dto.DaysUntilTourStart = (int)dto.TimeUntilTourStart.Value.TotalDays;
        dto.HoursUntilTourStart = (int)dto.TimeUntilTourStart.Value.TotalHours;
        dto.IsTourStartingSoon = dto.DaysUntilTourStart <= 7 && dto.DaysUntilTourStart >= 0;

        // محاسبه فیلدهای ثبت‌نام
        var activeCapacities = tour.GetActiveCapacities().ToList();
        if (activeCapacities.Any())
        {
            dto.RegistrationStart = activeCapacities.Min(c => c.RegistrationStart);
            dto.RegistrationEnd = activeCapacities.Max(c => c.RegistrationEnd);
            dto.TimeUntilRegistrationEnd = dto.RegistrationEnd - currentTime;
            dto.IsRegistrationEndingSoon = dto.TimeUntilRegistrationEnd.HasValue && 
                                         dto.TimeUntilRegistrationEnd.Value.TotalDays <= 3 && 
                                         dto.TimeUntilRegistrationEnd.Value.TotalDays >= 0;
        }

        // محاسبه متن‌های وضعیت
        dto.TourStatusText = GetTourStatusText(tour.Status);
        dto.RegistrationStatusText = GetRegistrationStatusText(tour, currentTime);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// محاسبه فیلدهای ظرفیت پیشرفته برای TourDto
    /// </summary>
    public async Task CalculateCapacityFieldsAsync(TourDto dto, Tour tour, CancellationToken cancellationToken = default)
    {
        // محاسبه تعداد شرکت‌کنندگان بر اساس وضعیت
        dto.ConfirmedParticipants = await _reservationRepository.GetConfirmedParticipantCountAsync(tour.Id, cancellationToken);
        dto.PendingParticipants = await _reservationRepository.GetPendingParticipantCountAsync(tour.Id, cancellationToken);
        
        // محاسبه کل استفاده از ظرفیت
        var totalUtilization = await _reservationRepository.GetTourUtilizationAsync(tour.Id, cancellationToken);
        dto.PayingParticipants = totalUtilization - dto.ConfirmedParticipants - dto.PendingParticipants;
        
        // محاسبه شرکت‌کنندگان منقضی شده (تقریبی)
        dto.ExpiredParticipants = Math.Max(0, tour.GetConfirmedReservationCount() + tour.GetPendingReservationCount() - totalUtilization);

        // محاسبه فیلدهای وضعیت ظرفیت
        dto.CapacityStatusText = GetCapacityStatusText(dto.RemainingCapacity, dto.MaxCapacity);
        dto.CapacityMessage = GetCapacityMessage(dto.RemainingCapacity, dto.MaxCapacity);
        dto.IsCapacityCritical = dto.RemainingCapacity <= 5 && dto.RemainingCapacity > 0;
        dto.AvailableSpotsForUser = dto.RemainingCapacity; // می‌تواند بر اساس محدودیت‌های کاربر تنظیم شود
    }

    /// <summary>
    /// محاسبه فیلدهای قیمت‌گذاری برای TourDto
    /// </summary>
    public void CalculatePricingFields(TourDto dto, Tour tour)
    {
        var activePricing = tour.GetActivePricing().ToList();
        if (activePricing.Any())
        {
            var prices = activePricing.Select(p => p.GetEffectivePrice().AmountRials).ToList();
            dto.LowestPriceRials = prices.Min();
            dto.HighestPriceRials = prices.Max();
            dto.AveragePriceRials = prices.Average();

            // بررسی تخفیف
            dto.HasDiscount = activePricing.Any(p => p.DiscountPercentage.HasValue && p.DiscountPercentage.Value > 0);
            var discountPercentages = activePricing.Where(p => p.DiscountPercentage.HasValue).Select(p => p.DiscountPercentage!.Value);
            dto.MaxDiscountPercentage = discountPercentages.Any() ? discountPercentages.Max() : (decimal?)null;

            // محاسبه متن محدوده قیمت
            if (dto.LowestPriceRials.HasValue && dto.HighestPriceRials.HasValue)
            {
                dto.PriceRangeText = GetPriceRangeText(dto.LowestPriceRials.Value, dto.HighestPriceRials.Value);
            }

            // بررسی پیش‌خرید و آخرین لحظه (فیلدهای موجود در TourPricing)
            dto.IsEarlyBirdAvailable = false; // TODO: اضافه کردن فیلد IsEarlyBird به TourPricing
            dto.IsLastMinuteAvailable = false; // TODO: اضافه کردن فیلد IsLastMinute به TourPricing
        }
    }

    /// <summary>
    /// محاسبه فیلدهای کاربری برای TourDto
    /// </summary>
    public void CalculateUserFields(TourDto dto, TourReservation? userReservation)
    {
        dto.HasUserReservation = userReservation != null;
        
        if (userReservation != null)
        {
            dto.UserReservationStatusText = GetReservationStatusText(userReservation.Status);
            dto.UserReservationExpiryDate = userReservation.ExpiryDate;
            
            if (userReservation.ExpiryDate.HasValue)
            {
                dto.TimeUntilUserReservationExpiry = userReservation.ExpiryDate.Value - DateTime.UtcNow;
            }

            // محاسبه اقدامات ممکن
            dto.CanUserCancel = ReservationStateMachine.CanBeCancelled(userReservation.Status);
            dto.CanUserModify = userReservation.IsActive() && (userReservation.Status == ReservationStatus.Held || userReservation.Status == ReservationStatus.Draft);
            dto.CanUserReserve = false; // اگر رزرو فعال دارد، نمی‌تواند دوباره رزرو کند

            // Domain behavior properties
            dto.IsUserReservationExpired = userReservation.IsExpired();
            dto.IsUserReservationActive = userReservation.IsActive();
            dto.IsUserReservationPending = userReservation.IsPending();
            dto.IsUserReservationConfirmed = userReservation.IsConfirmed();
            dto.IsUserReservationCancelled = userReservation.IsCancelled();
            dto.IsUserReservationTerminal = userReservation.IsTerminal();
        }
        else
        {
            dto.CanUserReserve = dto.IsRegistrationOpen && dto.RemainingCapacity > 0 && dto.IsActive;
            dto.CanUserCancel = false;
            dto.CanUserModify = false;
        }

        // محاسبه دلایل عدم امکان رزرو
        dto.ReservationBlockReasons = GetReservationBlockReasons(dto, userReservation);
    }

    /// <summary>
    /// محاسبه فیلدهای آماری برای TourDto
    /// </summary>
    public async Task CalculateStatisticsFieldsAsync(TourDto dto, Tour tour, CancellationToken cancellationToken = default)
    {
        var allReservations = await _reservationRepository.GetByTourIdAsync(tour.Id, cancellationToken);
        var reservationsList = allReservations.ToList();

        dto.TotalReservationsCount = reservationsList.Count;
        dto.ConfirmedReservationsCount = reservationsList.Count(r => r.Status == ReservationStatus.Confirmed);
        dto.PendingReservationsCount = reservationsList.Count(r => r.Status == ReservationStatus.Held || r.Status == ReservationStatus.Paying);
        dto.CancelledReservationsCount = reservationsList.Count(r => r.Status == ReservationStatus.Cancelled);

        // محاسبه نرخ موفقیت
        var totalAttempts = dto.TotalReservationsCount;
        dto.ReservationSuccessRate = totalAttempts > 0 ? (double)dto.ConfirmedReservationsCount / totalAttempts * 100 : 0;

        // محاسبه محبوبیت (بر اساس تعداد رزروها)
        dto.IsPopular = dto.TotalReservationsCount > 50;
        dto.IsTrending = dto.ReservationSuccessRate > 80 && dto.TotalReservationsCount > 20;

        // ViewCount و LastViewedAt باید از جداول مربوطه محاسبه شوند
        dto.ViewCount = 0; // TODO: محاسبه از جدول ViewCount
        dto.LastViewedAt = null; // TODO: محاسبه از جدول ViewCount
    }

    #region Helper Methods

    private static string GetTourStatusText(TourStatus status)
    {
        return status switch
        {
            TourStatus.Draft => "پیش‌نویس",
            TourStatus.Cancelled => "لغو شده",
            TourStatus.Completed => "تکمیل شده",
            _ => "نامشخص"
        };
    }

    private static string GetRegistrationStatusText(Tour tour, DateTime currentTime)
    {
        if (!tour.IsActive)
            return "تور غیرفعال است";

        var activeCapacities = tour.GetActiveCapacities().ToList();
        if (!activeCapacities.Any())
            return "ظرفیت ثبت‌نام موجود نیست";

        var registrationStart = activeCapacities.Min(c => c.RegistrationStart);
        var registrationEnd = activeCapacities.Max(c => c.RegistrationEnd);

        if (currentTime < registrationStart)
            return "ثبت‌نام هنوز شروع نشده است";
        
        if (currentTime > registrationEnd)
            return "ثبت‌نام به پایان رسیده است";

        return "ثبت‌نام باز است";
    }

    private static string GetCapacityStatusText(int remainingCapacity, int maxCapacity)
    {
        if (remainingCapacity <= 0)
            return "ظرفیت تکمیل شده";
        
        if (remainingCapacity <= 5)
            return "ظرفیت محدود";
        
        if (remainingCapacity <= maxCapacity * 0.2)
            return "ظرفیت کم";
        
        return "ظرفیت کافی";
    }

    private static string GetCapacityMessage(int remainingCapacity, int maxCapacity)
    {
        if (remainingCapacity <= 0)
            return "متأسفانه ظرفیت این تور تکمیل شده است";
        
        if (remainingCapacity <= 5)
            return $"فقط {remainingCapacity} جای خالی باقی مانده است";
        
        if (remainingCapacity <= maxCapacity * 0.2)
            return "ظرفیت محدودی باقی مانده است";
        
        return "ظرفیت کافی برای ثبت‌نام موجود است";
    }

    private static string GetPriceRangeText(decimal lowestPrice, decimal highestPrice)
    {
        if (lowestPrice == highestPrice)
            return $"{lowestPrice:N0} ریال";
        
        return $"{lowestPrice:N0} تا {highestPrice:N0} ریال";
    }

    private static string GetReservationStatusText(ReservationStatus status)
    {
        return status switch
        {
            ReservationStatus.Draft => "پیش‌نویس",
            ReservationStatus.Held => "رزرو شده",
            ReservationStatus.Paying => "در حال پرداخت",
            ReservationStatus.Confirmed => "تأیید شده",
            ReservationStatus.Expired => "منقضی شده",
            ReservationStatus.PaymentFailed => "پرداخت ناموفق",
            ReservationStatus.Cancelled => "لغو شده",
            ReservationStatus.SystemCancelled => "لغو شده توسط سیستم",
            ReservationStatus.Waitlisted => "در لیست انتظار",
            ReservationStatus.Refunding => "در حال بازپرداخت",
            ReservationStatus.Refunded => "بازپرداخت شده",
            ReservationStatus.NoShow => "عدم حضور",
            ReservationStatus.CancelRequested => "درخواست لغو",
            ReservationStatus.AmendRequested => "درخواست تغییر",
            ReservationStatus.Rejected => "رد شده",
            _ => "نامشخص"
        };
    }

    private static List<string> GetReservationBlockReasons(TourDto dto, TourReservation? userReservation)
    {
        var reasons = new List<string>();

        if (userReservation != null && userReservation.IsActive())
        {
            reasons.Add("شما قبلاً برای این تور رزرو فعال دارید");
            return reasons;
        }

        if (!dto.IsActive)
            reasons.Add("تور غیرفعال است");

        if (!dto.IsRegistrationOpen)
            reasons.Add("زمان ثبت‌نام به پایان رسیده است");

        if (dto.RemainingCapacity <= 0)
            reasons.Add("ظرفیت تور تکمیل شده است");

        if (dto.MinAge.HasValue || dto.MaxAge.HasValue)
            reasons.Add("محدودیت سنی وجود دارد");

        return reasons;
    }

    #endregion
}
