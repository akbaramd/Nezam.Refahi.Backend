using System.Text.Json.Serialization;

namespace Nezam.Refahi.Recreation.Domain.Enums;

/// <summary>
/// Represents the status of a tour
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TourDifficulty
{
  Easy = 0,        // مسیر آسان، قابل انجام برای عموم
  Moderate = 1,    // نیازمند آمادگی متوسط
  Hard = 2,        // مسیر دشوار، برای افراد با تجربه
  VeryHard = 3,    // چالش‌برانگیز، نیاز به آمادگی بالا
  Extreme = 4      // سطح بسیار سخت یا فنی (کوهستان، کویر، ارتفاع)
}