using System.Text.Json.Serialization;

namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// وضعیت پرداخت (بانک پارسیان و کیف پول داخلی)
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
  /// <summary>
  /// پرداخت ایجاد شده ولی هنوز آغاز نشده (در انتظار)
  /// </summary>
  Pending = 1,

  /// <summary>
  /// پرداخت در حال انجام در درگاه بانکی یا در فرآیند تایید
  /// </summary>
  Processing = 2,

  /// <summary>
  /// پرداخت با موفقیت انجام شده (از درگاه بانکی)
  /// </summary>
  Completed = 3,

  /// <summary>
  /// پرداخت از طریق کیف پول داخلی با موفقیت انجام شده
  /// </summary>
  PaidFromWallet = 4,

  /// <summary>
  /// پرداخت ناموفق یا رد شده از سمت بانک
  /// </summary>
  Failed = 5,

  /// <summary>
  /// پرداخت توسط کاربر لغو شده است
  /// </summary>
  Cancelled = 6,

  /// <summary>
  /// پرداخت به علت اتمام زمان مجاز منقضی شده است
  /// </summary>
  Expired = 7,

  /// <summary>
  /// پرداخت بازگشت داده شده (Refund)
  /// </summary>
  Refunded = 8
}
