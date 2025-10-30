using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Domain.Repositories;

/// <summary>
/// Repository برای مدیریت درخواست‌های تسهیلات
/// </summary>
public interface IFacilityRequestRepository : IRepository<FacilityRequest, Guid>
{
    /// <summary>
    /// دریافت درخواست‌های یک کاربر
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های یک دوره
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetByCycleIdAsync(Guid cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های یک تسهیلات
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های بر اساس وضعیت
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetByStatusAsync(FacilityRequestStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های در انتظار پردازش بانک
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetPendingBankProcessingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های با قرار ملاقات بانک
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetWithBankAppointmentsAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های تکمیل شده
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetCompletedApplicationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های رد شده
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetRejectedApplicationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود درخواست فعال برای کاربر
    /// </summary>
    Task<bool> HasActiveApplicationAsync(Guid userId, Guid facilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های بر اساس شماره درخواست
    /// </summary>
    Task<FacilityRequest?> GetByApplicationNumberAsync(string applicationNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های بر اساس IdempotencyKey
    /// </summary>
    Task<FacilityRequest?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های بر اساس CorrelationId
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های بر اساس بازه زمانی
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت آمار درخواست‌ها
    /// </summary>
    Task<ApplicationStatistics> GetApplicationStatisticsAsync(Guid? facilityId = null, Guid? cycleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌ها با صفحه‌بندی و فیلتر
    /// </summary>
    Task<IEnumerable<FacilityRequest>> GetFacilityRequestsAsync(FacilityRequestQueryParameters parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت تعداد درخواست‌ها با فیلتر
    /// </summary>
    Task<int> GetFacilityRequestsCountAsync(FacilityRequestQueryParameters parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت جزئیات درخواست با داده‌های مرتبط
    /// </summary>
    Task<FacilityRequest?> GetByIdWithDetailsAsync(Guid requestId, bool includeFacility = true, bool includeCycle = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود درخواست کاربر برای یک دوره خاص
    /// </summary>
    Task<bool> HasUserRequestForCycleAsync(Guid userId, Guid cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست کاربر برای یک دوره خاص
    /// </summary>
    Task<FacilityRequest?> GetUserLastRequestForCycleAsync(Guid userId, Guid cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخواست‌های کاربر برای چندین دوره (بهینه‌سازی شده)
    /// </summary>
    Task<Dictionary<Guid, FacilityRequest>> GetUserRequestsForCycleAsync(Guid userId, Guid cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود درخواست کاربر برای چندین دوره (بهینه‌سازی شده)
    /// </summary>
    Task<HashSet<Guid>> GetCyclesWithUserRequestsAsync(Guid userId, IEnumerable<Guid> cycleIds, CancellationToken cancellationToken = default);
}

/// <summary>
/// آمار درخواست‌ها
/// </summary>
public class ApplicationStatistics
{
    public int TotalApplications { get; set; }
    public int SubmittedApplications { get; set; }
    public int UnderReviewApplications { get; set; }
    public int ApprovedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public int CancelledApplications { get; set; }
    public int CompletedApplications { get; set; }
    public int PendingBankProcessing { get; set; }
    public decimal TotalRequestedAmount { get; set; }
    public decimal TotalApprovedAmount { get; set; }
}

/// <summary>
/// پارامترهای جستجوی درخواست‌های تسهیلات
/// </summary>
public class FacilityRequestQueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? FacilityId { get; set; }
    public Guid? FacilityCycleId { get; set; }
    public Guid? MemberId { get; set; } // Changed from ExternalUserId to MemberId
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
