using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// تسهیلات - تعریف نوع تسهیلات و سیاست‌های کلی
/// در دنیای واقعی: وام، کمک‌هزینه، کارت، بن رفاهی و سایر تسهیلات سازمان
/// </summary>
public sealed class Facility : FullAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    
    // Bank information
    public string? BankName { get; private set; }
    public string? BankCode { get; private set; }
    public string? BankAccountNumber { get; private set; }

    // Navigation property for EF Core querying (read-only)
    // Note: This is for querying purposes only; Facility and FacilityCycle are separate aggregates
    public ICollection<FacilityCycle> Cycles { get; private set; } = new List<FacilityCycle>();

    // Private constructor for EF Core
    private Facility() : base() { }

    /// <summary>
    /// ایجاد تسهیلات جدید
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک تسهیلات جدید با اطلاعات ابتدایی ایجاد می‌کند که شامل نام، کد،
    /// توضیحات و اطلاعات بانک است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که سازمان نیاز به تعریف نوع جدیدی از تسهیلات دارد، این رفتار برای
    /// ایجاد تعریف اولیه تسهیلات استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - نام و کد تسهیلات اجباری و یکتا باشد.
    /// - توضیحات و اطلاعات بانک اختیاری است.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: نام و کد معتبر ارائه شود.
    /// - نباید: کد تکراری استفاده شود.
    /// </remarks>
    public Facility(
        string name,
        string code,
        string? description = null,
        string? bankName = null,
        string? bankCode = null,
        string? bankAccountNumber = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        Name = name.Trim();
        Code = code.Trim();
        Description = description?.Trim();
        
        // Bank information
        BankName = bankName?.Trim();
        BankCode = bankCode?.Trim();
        BankAccountNumber = bankAccountNumber?.Trim();

        // Raise domain event
        AddDomainEvent(new FacilityCreatedEvent(
            Id,
            Name,
            Code,
            CreatedAt));
    }

    /// <summary>
    /// به‌روزرسانی اطلاعات وام
    /// </summary>
    public void UpdateInfo(
        string? name = null,
        string? description = null,
        string? bankName = null,
        string? bankCode = null,
        string? bankAccountNumber = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name.Trim();
        
        if (description != null)
            Description = description.Trim();
        
        if (bankName != null)
            BankName = bankName.Trim();
        
        if (bankCode != null)
            BankCode = bankCode.Trim();
        
        if (bankAccountNumber != null)
            BankAccountNumber = bankAccountNumber.Trim();
    }

}
