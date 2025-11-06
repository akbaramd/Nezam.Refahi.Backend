using MCA.SharedKernel.Domain;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// گزینه قیمت در لیست قیمت دوره تسهیلات
/// هر دوره می‌تواند چندین گزینه قیمت داشته باشد که کاربر از بین آنها انتخاب می‌کند
/// </summary>
public sealed class FacilityCyclePriceOption : Entity<Guid>
{
    public Guid FacilityCycleId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public FacilityCycle FacilityCycle { get; private set; } = null!;

    // Private constructor for EF Core
    private FacilityCyclePriceOption() : base() { }

    /// <summary>
    /// ایجاد گزینه قیمت جدید
    /// </summary>
    public FacilityCyclePriceOption(
        Guid facilityCycleId,
        Money amount,
        int displayOrder = 0,
        string? description = null)
        : base(Guid.NewGuid())
    {
        if (facilityCycleId == Guid.Empty)
            throw new ArgumentException("Facility cycle ID cannot be empty", nameof(facilityCycleId));
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        FacilityCycleId = facilityCycleId;
        Amount = amount;
        DisplayOrder = displayOrder;
        Description = description?.Trim();
        IsActive = true;
    }

    /// <summary>
    /// غیرفعال کردن گزینه قیمت
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// فعال کردن گزینه قیمت
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// به‌روزرسانی مبلغ
    /// </summary>
    public void UpdateAmount(Money newAmount)
    {
        if (newAmount == null)
            throw new ArgumentNullException(nameof(newAmount));
        if (newAmount.AmountRials <= 0)
            throw new ArgumentException("Amount must be positive", nameof(newAmount));

        Amount = newAmount;
    }

    /// <summary>
    /// به‌روزرسانی ترتیب نمایش
    /// </summary>
    public void UpdateDisplayOrder(int newOrder)
    {
        DisplayOrder = newOrder;
    }
}

