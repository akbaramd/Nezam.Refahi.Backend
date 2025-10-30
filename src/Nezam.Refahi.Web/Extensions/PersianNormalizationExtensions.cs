// File: Infrastructure/Extensions/PersianNormalizationExtensions.cs
namespace Nezam.New.EES.Extensions;

public static class PersianNormalizationExtensions
{
  /// <summary>
  /// جایگزینی ی عربی/ک عربی و … ، حذف فاصله‌های ابتدا-انتها و
  /// یکدست‌سازی نیم‌فاصله‌ها. اگر ورودی null یا whitespace باشد همان را برمی‌گرداند.
  /// </summary>
  public static string NormalizePersian(this string? input)
  {
    if (string.IsNullOrWhiteSpace(input))
      return input ?? string.Empty;

    return input
      .Trim()                        // حذف فاصله‌های اضافه
      .Replace('ي', 'ی')             // Yeh
      .Replace('ی', 'ی')             // variant Yeh
      .Replace('ك', 'ک')             // Keheh
      .Replace('ة', 'ه')             // Ta marbuta → Heh
      .Replace('ؤ', 'و')             // Waw with hamza → Waw
      .Replace('إ', 'ا')             // Alef with hamza below → Alef
      .Replace('أ', 'ا')             // Alef with hamza above → Alef
      .Replace('ٱ', 'ا')             // Alef wasla → Alef
      .Replace('ئ', 'ی')             // Yeh with hamza → Yeh
      .Replace("\u200C", "‌");       // همه نیم‌فاصله‌ها را به کد واحد U+200C ببرید
  }
}
