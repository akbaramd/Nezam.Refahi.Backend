using System.Text.Json.Serialization;

namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// درگاه‌های پرداخت پشتیبانی‌شده
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentGateway
{
  /// <summary>
  /// درگاه بانک پارسیان (اصلی سیستم)
  /// </summary>
  System = 1,

  /// <summary>
  /// درگاه زرین‌پال
  /// </summary>
  Zarinpal = 2,

  /// <summary>
  /// درگاه بانک ملت
  /// </summary>
  Mellat = 3,

  /// <summary>
  /// درگاه بانک سامان
  /// </summary>
  Saman = 4,

  /// <summary>
  /// درگاه بانک پاسارگاد
  /// </summary>
  Pasargad = 5,

  /// <summary>
  /// پرداخت از طریق کیف پول داخلی (بدون درگاه خارجی)
  /// </summary>
  Parsian = 6
}
