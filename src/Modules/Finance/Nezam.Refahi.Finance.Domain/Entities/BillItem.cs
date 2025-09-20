using MCA.SharedKernel.Domain;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// قلم صورت حساب - نماینده یک ردیف کالا یا خدمت در فاکتور
/// در دنیای واقعی: هر کالا، خدمت یا فعالیت قابل فروش با قیمت، تعداد و تخفیف
/// </summary>
public sealed class BillItem : Entity<Guid>
{
    public Guid BillId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal? DiscountPercentage { get; private set; }
    public Money LineTotal { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public Bill Bill { get; private set; } = null!;

    // Private constructor for EF Core
    private BillItem() : base() { }

    /// <summary>
    /// ایجاد قلم جدید برای صورت حساب
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک ردیف جدید کالا یا خدمت به صورت حساب اضافه می‌کند و به صورت خودکار
    /// مبلغ کل ردیف را بر اساس قیمت واحد، تعداد و تخفیف محاسبه می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که در فروشگاه کالایی به سبد خرید اضافه می‌شود یا در دفتر خدماتی
    /// به فاکتور مشتری افزوده می‌گردد، این رفتار اجرا شده و جزئیات آن ثبت می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - عنوان کالا یا خدمت اجباری است.
    /// - قیمت واحد باید معتبر و مثبت باشد.
    /// - تعداد باید بزرگتر از صفر باشد.
    /// - درصد تخفیف در صورت وجود باید بین 0 تا 100 باشد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: عنوان واضح و قیمت صحیح ثبت شود.
    /// - باید: محاسبه مبلغ کل به صورت خودکار انجام شود.
    /// - نباید: تعداد صفر یا منفی پذیرفته شود.
    /// - نباید: درصد تخفیف خارج از بازه مجاز باشد.
    /// </remarks>
    public BillItem(
        Guid billId,
        string title,
        string? description,
        Money unitPrice,
        int quantity = 1,
        decimal? discountPercentage = null)
        : base(Guid.NewGuid())
    {
        if (billId == Guid.Empty)
            throw new ArgumentException("Bill ID cannot be empty", nameof(billId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        if (discountPercentage.HasValue && (discountPercentage < 0 || discountPercentage > 100))
            throw new ArgumentException("Discount percentage must be between 0 and 100", nameof(discountPercentage));

        BillId = billId;
        Title = title.Trim();
        Description = description?.Trim();
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        Quantity = quantity;
        DiscountPercentage = discountPercentage;
        CreatedAt = DateTime.UtcNow;

        CalculateLineTotal();
    }

    /// <summary>
    /// دریافت مبلغ کل این ردیف صورت حساب
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار مبلغ نهایی محاسبه شده برای این ردیف از صورت حساب را برمی‌گرداند
    /// که شامل قیمت واحد ضرب در تعداد منهای مبلغ تخفیف اعمال شده است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به نمایش مبلغ نهایی هر ردیف در فاکتور، محاسبه مجموع کل صورت حساب
    /// یا گزارش‌گیری از فروش باشد، این رفتار استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - مبلغ کل برابر با (قیمت واحد × تعداد) - مبلغ تخفیف است.
    /// - نتیجه همیشه غیرمنفی خواهد بود.
    /// - محاسبات از قبل در هنگام ایجاد انجام شده است.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: مبلغ محاسبه شده از قبل برگردانده شود.
    /// - باید: نتیجه با محاسبه اولیه سازگار باشد.
    /// - نباید: محاسبه مجدد در هر بار فراخوانی انجام شود.
    /// </remarks>
    public Money GetTotalAmount()
    {
        return LineTotal;
    }

    // Private methods
    private void CalculateLineTotal()
    {
        var subtotal = UnitPrice.AmountRials * Quantity;
        decimal discountAmount = 0;

        if (DiscountPercentage.HasValue)
        {
            discountAmount = (subtotal * (DiscountPercentage.Value / 100));
        }

        LineTotal = Money.FromRials(subtotal - discountAmount);
    }
}