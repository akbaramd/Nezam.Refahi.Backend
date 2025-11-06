using MassTransit;
using System;
using Nezam.Refahi.Contracts.Finance.v1.Messages;

namespace Nezam.Refahi.Orchestrator.Sagas.WalletDeposit;

/// <summary>
/// وضعیت ساگا برای ارکستره کردن فرآیند «واریز به کیف پول»
/// مسیر ورود: WalletDepositRequestedEventMessage → ارسال IssueBillCommandMessage جهت ایجاد صورت‌حساب
/// ادامه فرایند: BillCreatedEventMessage (با ReferenceType = WalletDeposit) → ارسال MarkWalletDepositAwaitingPaymentCommandMessage
/// سپس با BillFullyPaidEventMessage → ارسال CompleteWalletDepositCommandMessage → انتشار WalletDepositCompletedEventMessage و اتمام ساگا
/// </summary>
public class WalletDepositSagaState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; } = string.Empty;

    // اطلاعات «درخواست واریز» که برای همبستگی و ادامه مسیر لازم است
    // CorrelationId: شناسهٔ یکتای این ساگا در جدول دیتابیس (کلید اصلی)
    public Guid WalletDepositId { get; set; }
    // شناسه کاربر بیرونی که قصد واریز دارد
    public Guid ExternalUserId { get; set; }
    // مبلغ به «ریال» (minor unit) برای حذف خطاهای اعشاری
    public decimal AmountRials { get; set; }
    // ارز مبلغ (به‌صورت پیش‌فرض IRR)
    public string Currency { get; set; } = "IRR";
    // کد رهگیری قابل‌نمایش به کاربر و برای گزارش‌گیری
    public string TrackingCode { get; set; } = string.Empty;
    // نام کامل کاربر برای درج در توضیحات فاکتور
    public string UserFullName { get; set; } = string.Empty;
    // توضیحات تکمیلی درخواست
    public string? Description { get; set; }
    // اطلاعات «فاکتور» (پس از ایجاد)
    public Guid? BillId { get; set; }
    public string? BillNumber { get; set; }
    // تاریخ‌های مهم برای مانیتورینگ و تحلیل
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? BillCreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // شناسه‌های «توکن زمان‌بندی» که MassTransit برای Unschedule به آن نیاز دارد (nullable)
    public Guid? BillCreationTimeoutTokenId { get; set; }
    public Guid? PaymentTimeoutTokenId { get; set; }
    // شمارندهٔ تلاش مجدد ایجاد فاکتور (برای کنترل backoff ساده)
    public int BillCreationRetryCount { get; set; }
}

public class WalletDepositSagaStateMachine : MassTransitStateMachine<WalletDepositSagaState>
{
    public WalletDepositSagaStateMachine()
    {
        // نگاشت وضعیت جاری ساگا به ستون CurrentState (برای کوئری و تحلیل)
        InstanceState(x => x.CurrentState);

        // رویداد «درخواست واریز»؛ همبستگی با WalletDepositId و تنظیم CorrelationId
        Event(() => WalletDepositRequested, x =>
        {
            x.CorrelateById(context => context.Message.WalletDepositId);
            x.SelectId(context => context.Message.WalletDepositId);
        });
        // رویداد «ایجاد فاکتور»؛ همبستگی با ReferenceId (Guid)
        // در صورت نبود اینستانس (مثلاً بعد از Finalize)، پیام نادیده گرفته می‌شود
        Event(() => BillCreated, x =>
        {
            x.CorrelateById(context => context.Message.ReferenceId);
            x.SelectId(context => context.Message.ReferenceId);
            x.OnMissingInstance(m => m.Discard());
        });
        // مسیرهای شکست برای ساده‌سازی حذف شدند
        // رویداد «واریز تکمیل شد» برای Finalize ساگا
        Event(() => DepositCompleted, x =>
        {
            x.CorrelateById(context => context.Message.WalletDepositId);
        });
        // رویداد «پرداخت کامل فاکتور»؛ پیام‌های دیررس در نبود اینستانس حذف می‌شوند
        Event(() => BillFullyPaid, x =>
        {
            x.CorrelateById(context => context.Message.ReferenceId);
            x.OnMissingInstance(m => m.Discard());
        });

        // زمان‌بندی‌ها حذف شده‌اند (صرفاً فیلدهای توکن در state برای سازگاری باقی مانده‌اند)

        // نقطه شروع ساگا: با دریافت «درخواست واریز»
        Initially(
            When(WalletDepositRequested)
                .Then(context =>
                {
                    // مقداردهی وضعیت داخلی ساگا از روی پیام ورودی
                    context.Saga.CorrelationId = context.Message.WalletDepositId;
                    context.Saga.WalletDepositId = context.Message.WalletDepositId;
                    context.Saga.ExternalUserId = context.Message.ExternalUserId;
                    context.Saga.AmountRials = context.Message.AmountRials;
                    context.Saga.Currency = context.Message.Currency ?? "IRR";
                    context.Saga.TrackingCode = context.Message.TrackingCode;
                    context.Saga.UserFullName = context.Message.UserFullName ?? string.Empty;
                    context.Saga.Description = context.Message.Description;
                    context.Saga.CreatedAt = context.Message.OccurredOn;
                    context.Saga.BillCreationRetryCount = 0;
                })
                .Send( FinanceMessagingRoutes.FinanceMessagesQueue,context =>
                {
                   // ارسال «دستور ایجاد فاکتور» به ماژول Finance
                  return new IssueBillCommandMessage
                  {
                    TrackingCode = context.Message.TrackingCode,
                    ReferenceId = context.Saga.WalletDepositId,
                    ReferenceType = FinanceMessagingRoutes.ReferenceTypes.WalletDeposit, // نوع مرجع جهت همبستگی در Finance
                    ExternalUserId = context.Message.ExternalUserId,
                    UserFullName = context.Message.UserFullName,
                    BillTitle = $"واریز کیف پول - {context.Message.UserFullName}",
                    Description = context.Message.Description ?? "ایجاد صورت‌حساب واریز کیف پول",
                    Metadata =
                      new Dictionary<string, string>()
                      {
                        ["WalletDepositId"] = context.Saga.WalletDepositId.ToString()
                      },
                    Items = new List<CreateBillItemMessage>
                    {
                      new CreateBillItemMessage
                      {
                        Title = "واریز کیف پول",
                        Description =
                          $"واریز کیف پول برای {context.Message.UserFullName} - کد پیگیری: {context.Message.TrackingCode}",
                        UnitPriceRials = context.Message.AmountRials,
                        Quantity = 1,
                        DiscountPercentage = null
                      }
                    }
                  };
                })
                .TransitionTo(AwaitingBillCreation)
        );

        // در حالت «در انتظار ایجاد فاکتور»
        During(AwaitingBillCreation,
            When(BillCreated, context =>
                context.Message.ReferenceType.Equals(FinanceMessagingRoutes.ReferenceTypes.WalletDeposit, StringComparison.OrdinalIgnoreCase) && // فقط فاکتورهای WalletDeposit
                context.Message.ReferenceId == context.Saga.WalletDepositId)
                .Then(context =>
                {
                    // ذخیرهٔ اطلاعات فاکتور در state
                    context.Saga.BillId = context.Message.BillId;
                    context.Saga.BillNumber = context.Message.BillNumber;
                    context.Saga.BillCreatedAt = context.Message.OccurredOn;
                })
                // زمان‌بندی ایجاد فاکتور فعال نیست
                .Send(FinanceMessagingRoutes.FinanceMessagesQueue, context =>
                {
                   // پس از ایجاد فاکتور، وضعیت «در انتظار پرداخت» در دامنه ثبت می‌شود
                  return new MarkWalletDepositAwaitingPaymentCommandMessage
                  {
                    WalletDepositId = context.Saga.WalletDepositId,
                    TrackingCode = context.Message.TrackingCode,
                    ExternalUserId = context.Message.ExternalUserId,
                    UserFullName = context.Message.UserFullName,
                    AmountRials = context.Message.TotalAmountRials,
                    Currency = context.Message.Currency,
                    BillId = context.Saga.BillId ?? context.Message.BillId,
                    BillNumber = context.Saga.BillNumber ?? context.Message.BillNumber
                  };
                })
                .TransitionTo(AwaitingPayment)
            // منطق TTL و مسیر شکست ایجاد فاکتور حذف شده است
        );

        // رویداد پرداخت کامل برای ادامه فرایند (شارژ کیف پول)
        // تعریف رویداد قبلاً انجام شده است

        // در حالت «در انتظار پرداخت»
        During(AwaitingPayment,
            When(BillFullyPaid, context =>
                context.Message.ReferenceType.Equals(FinanceMessagingRoutes.ReferenceTypes.WalletDeposit, StringComparison.OrdinalIgnoreCase) && // فقط رخدادهای مربوط به WalletDeposit
                context.Message.ReferenceId == context.Saga.WalletDepositId)
                // زمان‌بندی پرداخت فعال نیست
                .Send(FinanceMessagingRoutes.FinanceMessagesQueue,context =>
                {
                   // تکمیل واریز و سپس شارژ کیف پول در دامنه
                  return new CompleteWalletDepositCommandMessage
                {
                    WalletDepositId = context.Saga.WalletDepositId,
                    TrackingCode = context.Message.ReferenceTrackingCode,
                    ExternalUserId = context.Message.ExternalUserId,
                    UserFullName = context.Message.UserFullName,
                    AmountRials = context.Message.PaidAmountRials,
                    Currency = context.Saga.Currency,
                    BillId = context.Saga.BillId ?? Guid.Empty,
                    BillNumber = context.Saga.BillNumber ?? string.Empty,
                    PaymentId = context.Message.PaymentId,
                    PaidAt = context.Message.PaidAt
                  };
                })
                .TransitionTo(AwaitingCompletion)
            // مسیرهای شکست و TTL حذف شده است
        );

        // در حالت «در انتظار نهایی‌سازی/شارژ»
        During(AwaitingCompletion,
            When(DepositCompleted)
                // ثبت زمان تکمیل و پایان دادن به ساگا
                .Then(ctx => { ctx.Saga.CompletedAt = ctx.Message.CompletedAt; })
                .Finalize()
        );

        SetCompletedWhenFinalized();

        // Ignoreها برای سادگی حذف شدند
    }

    public State AwaitingBillCreation { get; private set; } = null!;
    public State AwaitingPayment { get; private set; } = null!;
    public State AwaitingCompletion { get; private set; } = null!;
    // حالت‌های Expired/Failed حذف شدند

    public Event<WalletDepositRequestedEventMessage> WalletDepositRequested { get; private set; } = null!;
    public Event<BillCreatedEventMessage> BillCreated { get; private set; } = null!;
    public Event<BillFullyPaidEventMessage> BillFullyPaid { get; private set; } = null!;
    // رویدادهای شکست حذف شدند
    public Event<WalletDepositCompletedEventMessage> DepositCompleted { get; private set; } = null!;

    // زمان‌بندها حذف شده‌اند
}



// Timeout message contracts (scheduler)
public class BillCreationTimeoutExpired
{
    public Guid CorrelationId { get; set; }
}

public class PaymentTtlExpired
{
    public Guid CorrelationId { get; set; }
}
