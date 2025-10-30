using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Events;

/// <summary>
/// رویداد ایجاد کد تخفیف جدید
/// </summary>
public sealed class DiscountCodeCreatedEvent(
    Guid DiscountCodeId,
    string Code,
    string Title,
    DiscountType Type,
    decimal DiscountValue,
    DateTime ValidFrom,
    DateTime ValidTo,
    int? UsageLimit,
    bool IsSingleUse,
    Guid? CreatedByExternalUserId,
    string? CreatedByUserFullName) : DomainEvent
{
    public Guid DiscountCodeId { get; } = DiscountCodeId;
    public string Code { get; } = Code;
    public string Title { get; } = Title;
    public DiscountType Type { get; } = Type;
    public decimal DiscountValue { get; } = DiscountValue;
    public DateTime ValidFrom { get; } = ValidFrom;
    public DateTime ValidTo { get; } = ValidTo;
    public int? UsageLimit { get; } = UsageLimit;
    public bool IsSingleUse { get; } = IsSingleUse;
    public Guid? CreatedByExternalUserId { get; } = CreatedByExternalUserId;
    public string? CreatedByUserFullName { get; } = CreatedByUserFullName;
}

/// <summary>
/// رویداد استفاده از کد تخفیف
/// </summary>
public sealed class DiscountCodeUsedEvent(
    Guid DiscountCodeId,
    string Code,
    Guid BillId,
    Guid ExternalUserId,
    string? UserFullName,
    Money BillAmount,
    Money DiscountAmount,
    int UsedCount) : DomainEvent
{
    public Guid DiscountCodeId { get; } = DiscountCodeId;
    public string Code { get; } = Code;
    public Guid BillId { get; } = BillId;
    public Guid ExternalUserId { get; } = ExternalUserId;
    public string? UserFullName { get; } = UserFullName;
    public Money BillAmount { get; } = BillAmount;
    public Money DiscountAmount { get; } = DiscountAmount;
    public int UsedCount { get; } = UsedCount;
}

/// <summary>
/// رویداد تغییر وضعیت کد تخفیف
/// </summary>
public sealed class DiscountCodeStatusChangedEvent(
    Guid DiscountCodeId,
    string Code,
    DiscountCodeStatus PreviousStatus,
    DiscountCodeStatus NewStatus,
    string? Reason) : DomainEvent
{
    public Guid DiscountCodeId { get; } = DiscountCodeId;
    public string Code { get; } = Code;
    public DiscountCodeStatus PreviousStatus { get; } = PreviousStatus;
    public DiscountCodeStatus NewStatus { get; } = NewStatus;
    public string? Reason { get; } = Reason;
}

/// <summary>
/// رویداد انقضای کد تخفیف
/// </summary>
public sealed class DiscountCodeExpiredEvent(
    Guid DiscountCodeId,
    string Code,
    DateTime ExpiredAt) : DomainEvent
{
    public Guid DiscountCodeId { get; } = DiscountCodeId;
    public string Code { get; } = Code;
    public DateTime ExpiredAt { get; } = ExpiredAt;
}

/// <summary>
/// رویداد اعمال کد تخفیف بر روی فاکتور
/// </summary>
public sealed class BillDiscountAppliedEvent(
    Guid BillId,
    string BillNumber,
    Guid DiscountCodeId,
    string DiscountCode,
    Money DiscountAmount,
    Money NewTotalAmount,
    Guid ExternalUserId,
    string? UserFullName) : DomainEvent
{
    public Guid BillId { get; } = BillId;
    public string BillNumber { get; } = BillNumber;
    public Guid DiscountCodeId { get; } = DiscountCodeId;
    public string DiscountCode { get; } = DiscountCode;
    public Money DiscountAmount { get; } = DiscountAmount;
    public Money NewTotalAmount { get; } = NewTotalAmount;
    public Guid ExternalUserId { get; } = ExternalUserId;
    public string? UserFullName { get; } = UserFullName;
}

/// <summary>
/// رویداد حذف کد تخفیف از فاکتور
/// </summary>
public sealed class BillDiscountRemovedEvent(
    Guid BillId,
    string BillNumber,
    Guid DiscountCodeId,
    string DiscountCode,
    Money RemovedDiscountAmount,
    Money NewTotalAmount,
    Guid ExternalUserId,
    string? UserFullName) : DomainEvent
{
    public Guid BillId { get; } = BillId;
    public string BillNumber { get; } = BillNumber;
    public Guid DiscountCodeId { get; } = DiscountCodeId;
    public string DiscountCode { get; } = DiscountCode;
    public Money RemovedDiscountAmount { get; } = RemovedDiscountAmount;
    public Money NewTotalAmount { get; } = NewTotalAmount;
    public Guid ExternalUserId { get; } = ExternalUserId;
    public string? UserFullName { get; } = UserFullName;
}
