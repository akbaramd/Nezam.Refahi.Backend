using System.Text.Json.Serialization;

namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// نوع روش پرداخت
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentMethod
{
  /// <summary>
  /// پرداخت آنلاین از طریق درگاه بانکی
  /// </summary>
  Online = 1,

  /// <summary>
  /// پرداخت از طریق کیف پول داخلی سیستم
  /// </summary>
  Wallet = 2,

  /// <summary>
  /// پرداخت نقدی در محل
  /// </summary>
  Cash = 3,

  /// <summary>
  /// کارت به کارت دستی (تأیید توسط کاربر یا ادمین)
  /// </summary>
  CardToCard = 4,

  /// <summary>
  /// انتقال بانکی (دستی یا از حساب سازمانی)
  /// </summary>
  BankTransfer = 5,
  Free = 5,
}
