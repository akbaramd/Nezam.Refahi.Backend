using MCA.SharedKernel.Domain;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// جدول رد درخواست‌های تسهیلات - برای ثبت جزئیات رد درخواست‌ها
/// </summary>
public sealed class FacilityRejection : Entity<Guid>
{
    public Guid RequestId { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? Details { get; private set; }
    public Guid RejectedByUserId { get; private set; }
    public string? RejectedByUserName { get; private set; }
    public DateTime RejectedAt { get; private set; }
    public FacilityRejectionType RejectionType { get; private set; }
    public string? Notes { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation property
    public FacilityRequest Request { get; private set; } = null!;

    // Private constructor for EF Core
    private FacilityRejection() : base() { }

    /// <summary>
    /// ایجاد رکورد رد درخواست
    /// </summary>
    public FacilityRejection(
        Guid requestId,
        string reason,
        Guid rejectedByUserId,
        FacilityRejectionType rejectionType = FacilityRejectionType.General,
        string? details = null,
        string? rejectedByUserName = null,
        string? notes = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (requestId == Guid.Empty)
            throw new ArgumentException("Request ID cannot be empty", nameof(requestId));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required", nameof(reason));
        if (rejectedByUserId == Guid.Empty)
            throw new ArgumentException("Rejected by user ID cannot be empty", nameof(rejectedByUserId));

        RequestId = requestId;
        Reason = reason.Trim();
        RejectedByUserId = rejectedByUserId;
        RejectedByUserName = rejectedByUserName?.Trim();
        RejectedAt = DateTime.UtcNow;
        RejectionType = rejectionType;
        Details = details?.Trim();
        Notes = notes?.Trim();
        
        if (metadata != null)
        {
            Metadata = new Dictionary<string, string>(metadata);
        }
    }

    /// <summary>
    /// به‌روزرسانی جزئیات رد
    /// </summary>
    public void UpdateDetails(string? details, string? notes = null)
    {
        Details = details?.Trim();
        Notes = notes?.Trim();
    }

    /// <summary>
    /// اضافه کردن متادیتا
    /// </summary>
    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be empty", nameof(key));
        
        Metadata[key] = value;
    }
}
