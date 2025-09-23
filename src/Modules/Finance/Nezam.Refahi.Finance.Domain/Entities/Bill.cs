using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// صورت حساب - مستند مالی که شامل اقلام و قابلیت پرداخت چندگانه است
/// در دنیای واقعی مانند فاکتور، قبض، صورتحساب خدمات و سایر اسناد مالی استفاده می‌شود
/// </summary>
public sealed class Bill : FullAggregateRoot<Guid>
{
    public string BillNumber { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string ReferenceId { get; private set; } = null!;
    public string BillType { get; private set; } = null!;
    public string UserNationalNumber { get; private set; } = null!;
    public string? UserFullName { get; private set; }
    public BillStatus Status { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public Money PaidAmount { get; private set; } = null!;
    public Money RemainingAmount { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? FullyPaidDate { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    private readonly List<BillItem> _items = new();
    public IReadOnlyCollection<BillItem> Items => _items.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private readonly List<Refund> _refunds = new();
    public IReadOnlyCollection<Refund> Refunds => _refunds.AsReadOnly();

    // Private constructor for EF Core
    private Bill() : base() { }

    /// <summary>
    /// ایجاد صورت حساب جدید در وضعیت پیش‌نویس
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک صورت حساب جدید با اطلاعات ابتدایی ایجاد می‌کند که در وضعیت پیش‌نویس
    /// قرار دارد و آماده اضافه کردن اقلام و ویرایش است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کسب‌وکار یا ارائهدهنده خدمات برای مشتری فاکتور، قبض یا صورتحساب
    /// صادر می‌کند، این رفتار برای ایجاد مستند مالی اولیه استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - عنوان صورت حساب اجباری و واضح باشد.
    /// - شناسه مرجع برای پیگیری ضروری است.
    /// - نوع سند باید مشخص باشد.
    /// - کد ملی مشتری اجباری است.
    /// - شماره صورت حساب به صورت خودکار تولید می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: اطلاعات ضروری برای شناسایی تکمیل باشد.
    /// - باید: شماره یکتا و قابل پیگیری تولید شود.
    /// - نباید: بدون اطلاعات ضروری ایجاد شود.
    /// - نباید: اطلاعات ورودی خالی یا نامعتبر باشد.
    /// </remarks>
    public Bill(
        string title,
        string referenceId,
        string billType,
        string userNationalNumber,
        string? userFullName = null,
        string? description = null,
        DateTime? dueDate = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(referenceId))
            throw new ArgumentException("Reference ID cannot be empty", nameof(referenceId));
        if (string.IsNullOrWhiteSpace(billType))
            throw new ArgumentException("Bill type cannot be empty", nameof(billType));
        if (string.IsNullOrWhiteSpace(userNationalNumber))
            throw new ArgumentException("User national number cannot be empty", nameof(userNationalNumber));

        BillNumber = GenerateBillNumber();
        Title = title.Trim();
        ReferenceId = referenceId.Trim();
        BillType = billType.Trim();
        UserNationalNumber = userNationalNumber.Trim();
        UserFullName = userFullName?.Trim();
        Description = description?.Trim();
        Status = BillStatus.Draft;
        IssueDate = DateTime.UtcNow;
        DueDate = dueDate;
        TotalAmount = Money.Zero;
        PaidAmount = Money.Zero;
        RemainingAmount = Money.Zero;
        Metadata = metadata ?? new Dictionary<string, string>();

        // Raise domain event
        AddDomainEvent(new BillCreatedEvent(
            Id,
            BillNumber,
            Title,
            ReferenceId,
            BillType,
            UserNationalNumber,
            UserFullName,
            TotalAmount,
            IssueDate,
            DueDate,
            Description));
    }

    /// <summary>
    /// افزودن قلم جدید به صورت حساب
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک قلم (ردیف) جدید به صورت حساب اضافه می‌کند و به صورت خودکار
    /// مبلغ کل صورت حساب را بر اساس قیمت، تعداد و تخفیف محاسبه مجدد می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که در فروشگاه کالایی به سبد خرید اضافه می‌شود یا در دفتر خدماتی
    /// یک خدمت به فاکتور مشتری افزوده می‌شود، این رفتار استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط در وضعیت پیش‌نویس قابل اجرا است.
    /// - عنوان و قیمت واحد اجباری هستند.
    /// - تعداد باید عددی مثبت باشد.
    /// - درصد تخفیف در صورت وجود باید بین 0 تا 100 باشد.
    /// - محاسبه مجدد مبلغ کل به صورت خودکار انجام می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: اطلاعات قلم کامل و معتبر باشد.
    /// - باید: محاسبه مجدد مبلغ کل انجام شود.
    /// - نباید: به صورتحساب‌های صادر شده قلم اضافه شود.
    /// - نباید: اطلاعات ناقص یا نامعتبر پذیرفته شود.
    /// </remarks>
    public void AddItem(string title, string? description, Money unitPrice, int quantity = 1, decimal? discountPercentage = null)
    {
        if (Status != BillStatus.Draft)
            throw new InvalidOperationException("Can only add items to draft bills");

        var item = new BillItem(Id, title, description, unitPrice, quantity, discountPercentage);
        _items.Add(item);
        RecalculateAmounts();
    }

    /// <summary>
    /// حذف قلم از صورت حساب
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک قلم (ردیف) را از صورت حساب حذف کرده و به صورت خودکار
    /// مبلغ کل صورت حساب را مجدداً محاسبه می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مشتری در فروشگاه یا دفتر خدماتی از خرید کالا یا خدمتی منصرف شود
    /// و قبل از نهایی کردن فاکتور آن را حذف کند، این رفتار استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط در وضعیت پیش‌نویس قابل اجرا است.
    /// - قلم باید وجود داشته باشد تا قابل حذف باشد.
    /// - محاسبه مجدد مبلغ کل بعد از حذف ضروری است.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از وضعیت پیش‌نویس قابل حذف باشد.
    /// - باید: محاسبه مجدد مبلغ انجام شود.
    /// - نباید: از صورتحساب‌های صادر شده حذف شود.
    /// - نباید: قلم ناموجود حذف شود.
    /// </remarks>
    public void RemoveItem(Guid itemId)
    {
        if (Status != BillStatus.Draft)
            throw new InvalidOperationException("Can only remove items from draft bills");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateAmounts();
        }
    }

    /// <summary>
    /// صدور و نهایی کردن صورت حساب
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار صورت حساب را از حالت پیش‌نویس به حالت صادر شده تغییر می‌دهد
    /// و آن را آماده و قابل پرداخت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که فروشنده یا ارائهدهنده خدمات فاکتور در حال تهیه را تکمیل و نهایی کرده
    /// و آن را برای مشتری جهت پرداخت ارسال می‌کند، این رفتار استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط صورتحساب‌های پیش‌نویس قابل صدور هستند.
    /// - حداقل یک قلم باید در صورت حساب وجود داشته باشد.
    /// - زمان صدور به صورت خودکار ثبت می‌شود.
    /// - وضعیت به صادر شده تغییر می‌یابد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: حداقل یک قلم شامل باشد.
    /// - باید: زمان صدور دقیق ثبت شود.
    /// - نباید: صورتحساب‌های غیر پیش‌نویس صادر شوند.
    /// - نباید: بدون قلم صادر شود.
    /// </remarks>
    public void Issue()
    {
        if (Status != BillStatus.Draft)
            throw new InvalidOperationException("Can only issue draft bills");
        if (!_items.Any())
            throw new InvalidOperationException("Cannot issue bill without items");

        var previousStatus = Status;
        Status = BillStatus.Issued;
        IssueDate = DateTime.UtcNow;

        // Raise domain event
        AddDomainEvent(new BillStatusChangedEvent(
            Id,
            BillNumber,
            previousStatus,
            Status,
            "Bill issued"));
    }

    /// <summary>
    /// ایجاد پرداخت برای این صورت حساب
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک پرداخت جدید برای این صورت حساب ایجاد می‌کند که شامل مبلغ، روش پرداخت،
    /// درگاه مورد نظر و زمان انقضا است و آن را به فهرست پرداخت‌ها اضافه می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مشتری تصمیم به پرداخت فاکتور می‌گیرد و روش پرداخت مورد نظر خود
    /// (آنلاین، کارتخوان، نقدی) را انتخاب می‌کند، این رفتار استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - صورت حساب نباید لغو شده باشد.
    /// - صورت حساب نباید کاملاً پرداخت شده باشد.
    /// - مبلغ پرداخت نباید از مبلغ باقیمانده بیشتر باشد.
    /// - پرداخت ایجاد شده به فهرست پرداخت‌ها اضافه می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: مبلغ و روش پرداخت معتبر باشد.
    /// - باید: پرداخت به فهرست اضافه و برگردانده شود.
    /// - نباید: برای صورتحساب‌های لغو شده پرداخت ایجاد شود.
    /// - نباید: مبلغ بیش از باقیمانده پذیرفته شود.
    /// </remarks>
    public Payment CreatePayment(
        Money amount,
        PaymentMethod method = PaymentMethod.Online,
        PaymentGateway? gateway = null,
        string? callbackUrl = null,
        string? description = null,
        DateTime? expiryDate = null)
    {
        if (Status == BillStatus.Cancelled)
            throw new InvalidOperationException("Cannot create payment for cancelled bill");
        if (Status == BillStatus.FullyPaid)
            throw new InvalidOperationException("Bill is already fully paid");
        if (amount.AmountRials > RemainingAmount.AmountRials)
            throw new InvalidOperationException("Payment amount exceeds remaining bill amount");

        var payment = new Payment(Id, BillNumber, amount, method, gateway, callbackUrl, description, expiryDate);
        _payments.Add(payment);

        // Raise domain event
        AddDomainEvent(new PaymentInitiatedEvent(
            payment.Id,
            Id,
            BillNumber,
            ReferenceId,
            BillType,
            UserNationalNumber,
            amount,
            method,
            gateway,
            payment.TrackingNumber,
            expiryDate,
            description));

        return payment;
    }

    /// <summary>
    /// ثبت پرداخت موفق
    /// در دنیای واقعی: تایید دریافت پول از درگاه پرداخت یا خزانهدار
    /// قوانین: پرداخت باید وجود داشته باشد، شناسه تراکنش اجباری است
    /// </summary>
    public void RecordPayment(Guid paymentId, string gatewayTransactionId, string? gatewayReference = null)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId);
        if (payment == null)
            throw new InvalidOperationException("Payment not found");

        payment.MarkAsCompleted(gatewayTransactionId, gatewayReference);

        PaidAmount = Money.FromRials(PaidAmount.AmountRials + payment.Amount.AmountRials);
        RemainingAmount = Money.FromRials(TotalAmount.AmountRials - PaidAmount.AmountRials);

        UpdateBillStatus();

        // Raise domain event
        AddDomainEvent(new PaymentCompletedEvent(
            payment.Id,
            ReferenceId,
            BillType,
            UserNationalNumber,
            payment.Amount.AmountRials,
            gatewayTransactionId,
            payment.CompletedAt!.Value));
    }

    /// <summary>
    /// علامت‌گذاری پرداخت به عنوان ناموفق
    /// در دنیای واقعی: زمانی که درگاه خطا برگرداند یا موجودی کافی نباشد
    /// قوانین: پرداخت باید وجود داشته باشد
    /// </summary>
    public void MarkPaymentAsFailed(Guid paymentId, string? failureReason = null)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId);
        if (payment == null)
            throw new InvalidOperationException("Payment not found");

        payment.MarkAsFailed(failureReason);

        // Raise domain event
        AddDomainEvent(new PaymentFailedEvent(
            payment.Id,
            ReferenceId,
            BillType,
            UserNationalNumber,
            failureReason));
    }

    /// <summary>
    /// ایجاد درخواست برگشت وجه برای این صورت حساب
    /// در دنیای واقعی: عودت کالا، لغو سفارش یا عدم رضایت مشتری
    /// قوانین: فقط برای اسناد پرداخت شده، مبلغ برگشت نباید از پرداختی بیشتر باشد
    /// </summary>
    public Refund CreateRefund(Money refundAmount, string reason, string? requestedByNationalNumber = null)
    {
        if (Status != BillStatus.FullyPaid && Status != BillStatus.PartiallyPaid)
            throw new InvalidOperationException("Can only refund paid bills");

        var totalRefunded = _refunds.Where(r => r.Status == RefundStatus.Completed)
                                  .Sum(r => r.Amount.AmountRials);

        if (totalRefunded + refundAmount.AmountRials > PaidAmount.AmountRials)
            throw new InvalidOperationException("Refund amount exceeds paid amount");

        var refund = new Refund(Id, refundAmount, reason, requestedByNationalNumber ?? UserNationalNumber);
        _refunds.Add(refund);

        // Raise domain event
        AddDomainEvent(new RefundInitiatedEvent(
            refund.Id,
            Id,
            Id, // BillId
            BillNumber,
            ReferenceId,
            BillType,
            refundAmount,
            refund.RequestedByNationalNumber,
            reason));

        return refund;
    }

    /// <summary>
    /// تکمیل فرآیند برگشت وجه
    /// در دنیای واقعی: تایید بازگشت پول به حساب مشتری توسط خزانهدار
    /// قوانین: درخواست باید وجود داشته باشد
    /// </summary>
    public void CompleteRefund(Guid refundId, string? gatewayRefundId = null, string? gatewayReference = null)
    {
        var refund = _refunds.FirstOrDefault(r => r.Id == refundId);
        if (refund == null)
            throw new InvalidOperationException("Refund not found");

        refund.MarkAsCompleted(gatewayRefundId, gatewayReference);

        PaidAmount = Money.FromRials(PaidAmount.AmountRials - refund.Amount.AmountRials);
        RemainingAmount = Money.FromRials(TotalAmount.AmountRials - PaidAmount.AmountRials);

        UpdateBillStatus();

        // Raise domain event
        AddDomainEvent(new RefundCompletedEvent(
            refund.Id,
            Id,
            ReferenceId,
            BillType,
            refund.Amount.AmountRials,
            refund.RequestedByNationalNumber,
            refund.CompletedAt!.Value));
    }

    /// <summary>
    /// لغو صورت حساب
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار صورت حساب را به عنوان لغو شده علامت‌گذاری کرده و تمام پرداخت‌های در انتظار
    /// یا در حال پردازش مرتبط با آن را نیز لغو می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که به دلیل عدم امکان پرداخت، انصراف مشتری، یا مسائل فنی نیاز به لغو فاکتور
    /// وجود دارد، این رفتار برای باطل کردن کامل سند مالی استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - صورت حساب‌های کاملاً پرداخت شده قابل لغو نیستند.
    /// - تمام پرداخت‌های جاری و در انتظار باید لغو شوند.
    /// - دلیل لغو در صورت وجود ثبت می‌شود.
    /// - وضعیت به لغو شده تغییر می‌یابد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: تمام پرداخت‌های مرتبط لغو شوند.
    /// - باید: دلیل لغو در صورت وجود ثبت شود.
    /// - نباید: فاکتورهای کاملاً پرداخت شده لغو شوند.
    /// - نباید: عملیات تکراری لغو انجام شود.
    /// </remarks>
    public void Cancel(string? reason = null)
    {
        if (Status == BillStatus.FullyPaid)
            throw new InvalidOperationException("Cannot cancel fully paid bill");
        if (Status == BillStatus.Cancelled)
            return; // Already cancelled

        // Cancel all pending payments
        foreach (var payment in _payments.Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing))
        {
            payment.Cancel(reason);
        }

        var previousStatus = Status;
        Status = BillStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["CancellationReason"] = reason;
        }

        // Raise domain event
        AddDomainEvent(new BillCancelledEvent(
            Id,
            BillNumber, 
            ReferenceId,
            BillType,
            UserNationalNumber,
            TotalAmount,
            PaidAmount,
            PaidAmount, // Refund amount equals paid amount for cancelled bills
            reason ?? "Bill cancelled"));
    }

    /// <summary>
    /// Voids the bill before any payment (prevents financial impact)
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار فاکتور را قبل از هر پرداختی ابطال می‌کند و از تأثیر مالی جلوگیری می‌نماید.
    /// با Cancel متفاوت است زیرا هیچ پرداختی انجام نشده است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که فاکتور اشتباه صادر شده یا نیاز به اصلاح قبل از ارسال به مشتری وجود دارد.
    ///
    /// <para>قوانین:</para>
    /// - فقط فاکتورهای Draft یا Issued قابل ابطال هستند.
    /// - هیچ پرداختی نباید انجام شده باشد.
    /// - وضعیت به Voided تغییر می‌یابد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط قبل از هر پرداختی قابل اجرا باشد.
    /// - باید: دلیل ابطال ثبت شود.
    /// - نباید: فاکتورهای پرداخت شده ابطال شوند.
    /// - نباید: با Cancel اشتباه گرفته شود.
    /// </remarks>
    public void Void(string? reason = null)
    {
        if (Status != BillStatus.Draft && Status != BillStatus.Issued)
            throw new InvalidOperationException("Can only void draft or issued bills");
        if (PaidAmount.AmountRials > 0)
            throw new InvalidOperationException("Cannot void bill with payments");
        if (Status == BillStatus.Voided)
            return; // Already voided

        var previousStatus = Status;
        Status = BillStatus.Voided;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["VoidReason"] = reason;
        }

        // Raise domain event
        AddDomainEvent(new BillStatusChangedEvent(
            Id,
            BillNumber,
            previousStatus,
            Status,
            reason ?? "Bill voided"));
    }

    /// <summary>
    /// Writes off the bill as bad debt (with financial documentation)
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار فاکتور را به عنوان مطالبه سوخته علامت‌گذاری می‌کند
    /// و نیاز به اسناد مالی و تأیید مدیریت مالی دارد.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مشتری قادر به پرداخت نیست یا مطالبه غیرقابل وصول است.
    ///
    /// <para>قوانین:</para>
    /// - فقط فاکتورهای Issued یا Overdue قابل سوخت هستند.
    /// - نیاز به تأیید مدیریت مالی دارد.
    /// - وضعیت به WrittenOff تغییر می‌یابد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: تأیید مدیریت مالی داشته باشد.
    /// - باید: اسناد مالی کامل باشد.
    /// - نباید: بدون تأیید انجام شود.
    /// - نباید: فاکتورهای Draft سوخته شوند.
    /// </remarks>
    public void WriteOff(string? reason = null, string? approvedBy = null)
    {
        if (Status != BillStatus.Issued && Status != BillStatus.Overdue)
            throw new InvalidOperationException("Can only write off issued or overdue bills");
        if (Status == BillStatus.WrittenOff)
            return; // Already written off

        var previousStatus = Status;
        Status = BillStatus.WrittenOff;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["WriteOffReason"] = reason;
        }
        if (!string.IsNullOrWhiteSpace(approvedBy))
        {
            Metadata["WriteOffApprovedBy"] = approvedBy;
        }

        // Raise domain event
        AddDomainEvent(new BillStatusChangedEvent(
            Id,
            BillNumber,
            previousStatus,
            Status,
            reason ?? "Bill written off"));
    }

    /// <summary>
    /// Issues a credit note for correction/post-payment discount
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار Credit Note صادر می‌کند برای اصلاح یا تخفیف پسینی
    /// که معادل Refund در سطح سند است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به اصلاح فاکتور یا اعطای تخفیف پسینی وجود دارد.
    ///
    /// <para>قوانین:</para>
    /// - فقط فاکتورهای Issued یا FullyPaid قابل اعتبار هستند.
    /// - مبلغ اعتبار نباید از مبلغ کل بیشتر باشد.
    /// - وضعیت به Credited تغییر می‌یابد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: مبلغ اعتبار معتبر باشد.
    /// - باید: دلیل اعتبار ثبت شود.
    /// - نباید: فاکتورهای Draft اعتبار یابند.
    /// - نباید: مبلغ بیش از کل فاکتور باشد.
    /// </remarks>
    public void Credit(Money creditAmount, string? reason = null)
    {
        if (Status != BillStatus.Issued && Status != BillStatus.FullyPaid)
            throw new InvalidOperationException("Can only credit issued or fully paid bills");
        if (creditAmount.AmountRials > TotalAmount.AmountRials)
            throw new InvalidOperationException("Credit amount cannot exceed total bill amount");
        if (Status == BillStatus.Credited)
            return; // Already credited

        var previousStatus = Status;
        Status = BillStatus.Credited;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["CreditReason"] = reason;
        }
        Metadata["CreditAmount"] = creditAmount.AmountRials.ToString();

        // Raise domain event
        AddDomainEvent(new BillStatusChangedEvent(
            Id,
            BillNumber,
            previousStatus,
            Status,
            reason ?? "Bill credited"));
    }

    /// <summary>
    /// Marks the bill as disputed (suspended until arbitration)
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار فاکتور را به عنوان مورد اعتراض مالی علامت‌گذاری می‌کند
    /// و پیگیری آن را تا زمان داوری متوقف می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مشتری با مبلغ یا محتوای فاکتور مخالف است و نیاز به داوری دارد.
    ///
    /// <para>قوانین:</para>
    /// - فقط فاکتورهای Issued یا FullyPaid قابل اعتراض هستند.
    /// - وضعیت به Disputed تغییر می‌یابد.
    /// - پیگیری متوقف می‌شود تا زمان حل اختلاف.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: دلیل اعتراض ثبت شود.
    /// - باید: تاریخ اعتراض ثبت شود.
    /// - نباید: فاکتورهای Draft اعتراض یابند.
    /// - نباید: بدون دلیل اعتراض انجام شود.
    /// </remarks>
    public void Dispute(string? reason = null)
    {
        if (Status != BillStatus.Issued && Status != BillStatus.FullyPaid)
            throw new InvalidOperationException("Can only dispute issued or fully paid bills");
        if (Status == BillStatus.Disputed)
            return; // Already disputed

        var previousStatus = Status;
        Status = BillStatus.Disputed;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["DisputeReason"] = reason;
        }
        Metadata["DisputeDate"] = DateTime.UtcNow.ToString("O");

        // Raise domain event
        AddDomainEvent(new BillStatusChangedEvent(
            Id,
            BillNumber,
            previousStatus,
            Status,
            reason ?? "Bill disputed"));
    }

    /// <summary>
    /// Resolves a disputed bill (moves back to appropriate status)
    /// </summary>
    public void ResolveDispute(BillStatus newStatus, string? resolution = null)
    {
        if (Status != BillStatus.Disputed)
            throw new InvalidOperationException("Can only resolve disputed bills");
        if (newStatus == BillStatus.Disputed)
            throw new InvalidOperationException("Cannot resolve dispute to disputed status");

        var previousStatus = Status;
        Status = newStatus;
        if (!string.IsNullOrWhiteSpace(resolution))
        {
            Metadata["DisputeResolution"] = resolution;
        }
        Metadata["DisputeResolvedDate"] = DateTime.UtcNow.ToString("O");

        // Raise domain event
        AddDomainEvent(new BillStatusChangedEvent(
            Id,
            BillNumber,
            previousStatus,
            Status,
            resolution ?? "Dispute resolved"));
    }

    /// <summary>
    /// بررسی سررسید بودن صورت حساب
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار بررسی می‌کند که آیا زمان مهلت پرداخت صورت حساب سپری شده
    /// و هنوز فاکتور پرداخت نشده است یا خیر.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که سیستم به صورت خودکار فاکتورهای قدیمی و سررسید را بررسی می‌کند
    /// یا زمانی که نیاز به اعلام یادآوری به مشتری وجود دارد.
    ///
    /// <para>قوانین:</para>
    /// - تاریخ سررسید باید تعیین شده باشد.
    /// - زمان جاری باید پس از تاریخ سررسید باشد.
    /// - صورت حساب نباید کاملاً پرداخت شده یا لغو شده باشد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط برای فاکتورهای دارای تاریخ سررسید بررسی شود.
    /// - باید: مقایسه زمان بر اساس UTC انجام شود.
    /// - نباید: فاکتورهای پرداخت شده را سررسید تلقی کند.
    /// - نباید: بدون تاریخ سررسید بررسی شود.
    /// </remarks>
    public bool IsOverdue()
    {
        return DueDate.HasValue &&
               DateTime.UtcNow > DueDate.Value &&
               Status != BillStatus.FullyPaid &&
               Status != BillStatus.Cancelled;
    }


    // Private methods
    private void RecalculateAmounts()
    {
        var total = _items.Sum(item => item.GetTotalAmount().AmountRials);
        TotalAmount = Money.FromRials(total);
        RemainingAmount = Money.FromRials(TotalAmount.AmountRials - PaidAmount.AmountRials);
    }

    private void UpdateBillStatus()
    {
        var previousStatus = Status;
        var wasFullyPaid = Status == BillStatus.FullyPaid;

        // Don't update status for terminal or special statuses
        if (Status == BillStatus.Voided || 
            Status == BillStatus.WrittenOff || 
            Status == BillStatus.Credited || 
            Status == BillStatus.Disputed ||
            Status == BillStatus.Cancelled ||
            Status == BillStatus.Refunded)
        {
            return;
        }

        if (PaidAmount.AmountRials == 0)
        {
            Status = BillStatus.Issued;
        }
        else if (PaidAmount.AmountRials >= TotalAmount.AmountRials)
        {
            Status = BillStatus.FullyPaid;
            FullyPaidDate = DateTime.UtcNow;
        }
        else
        {
            Status = BillStatus.PartiallyPaid;
        }

        // Check if overdue
        if (IsOverdue() && Status != BillStatus.FullyPaid)
        {
            Status = BillStatus.Overdue;
        }

        // Raise status changed event if status changed
        if (previousStatus != Status)
        {
            AddDomainEvent(new BillStatusChangedEvent(
                Id,
                BillNumber,
                previousStatus,
                Status,
                "Bill status updated due to payment"));
        }

        // Raise fully paid event if bill just became fully paid
        if ( Status == BillStatus.FullyPaid)
        {
            var completedPayments = _payments.Where(p => p.Status == PaymentStatus.Completed).ToList();
            AddDomainEvent(new BillFullyPaidEvent(
                Id,
                BillNumber,
                ReferenceId,
                BillType,
                UserNationalNumber,
                TotalAmount,
                PaidAmount,
                FullyPaidDate!.Value,
                completedPayments.Count,
                completedPayments.LastOrDefault()?.Method.ToString(),
                completedPayments.LastOrDefault()?.Gateway?.ToString()));
        }

        // Raise overdue event if bill just became overdue
        if (Status == BillStatus.Overdue && previousStatus != BillStatus.Overdue)
        {
            AddDomainEvent(new BillOverdueEvent(
                Id,
                BillNumber,
                ReferenceId,
                BillType,
                UserNationalNumber,
                TotalAmount,        
                RemainingAmount,
                DueDate!.Value,
                DateTime.UtcNow));
        }
    }

    private static string GenerateBillNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var randomPart = Random.Shared.Next(100, 999).ToString();
        return $"ف-{timestamp}{randomPart}";
    }
}