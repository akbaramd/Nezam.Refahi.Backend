using System.Globalization;

namespace Nezam.New.EES.Utilities;

  public static class PersianDateConverter
  {
      public static string ToPersianDate(this DateTime? dateTime, string format = "yyyy/MM/dd")
      {
          if (!dateTime.HasValue)
              return "-";
              
          return ToPersianDate(dateTime.Value, format);
      }
      
      public static string ToPersianDate(this DateTime dateTime, string format = "yyyy/MM/dd")
      {
          PersianCalendar pc = new PersianCalendar();
          
          int year = pc.GetYear(dateTime);
          int month = pc.GetMonth(dateTime);
          int day = pc.GetDayOfMonth(dateTime);
          int hour = pc.GetHour(dateTime);
          int minute = pc.GetMinute(dateTime);
          int second = pc.GetSecond(dateTime);
          
          string result = format;
          result = result.Replace("yyyy", year.ToString("0000"));
          result = result.Replace("yy", year.ToString("00"));
          result = result.Replace("MM", month.ToString("00"));
          result = result.Replace("M", month.ToString("0"));
          result = result.Replace("dd", day.ToString("00"));
          result = result.Replace("d", day.ToString("0"));
          result = result.Replace("HH", hour.ToString("00"));
          result = result.Replace("H", hour.ToString("0"));
          result = result.Replace("mm", minute.ToString("00"));
          result = result.Replace("m", minute.ToString("0"));
          result = result.Replace("ss", second.ToString("00"));
          result = result.Replace("s", second.ToString("0"));
          
          return result;
      }
      
      public static string GetPersianMonthName(int month)
      {
          string[] monthNames = new[]
          {
              "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
              "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
          };
          
          if (month < 1 || month > 12)
              return "";
              
          return monthNames[month - 1];
      }
      
      public static string GetPersianDayOfWeek(DateTime date)
      {
          string[] dayNames = new[]
          {
              "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنج‌شنبه", "جمعه", "شنبه"
          };
          
          int dayOfWeek = (int)date.DayOfWeek;
          return dayNames[dayOfWeek];
      }
      
      public static string ToPersianDateTextual(this DateTime? dateTime)
      {
          if (!dateTime.HasValue)
              return "-";
              
          return ToPersianDateTextual(dateTime.Value);
      }
      
      public static string ToPersianDateTextual(this DateTime dateTime)
      {
          PersianCalendar pc = new PersianCalendar();
          
          int year = pc.GetYear(dateTime);
          int month = pc.GetMonth(dateTime);
          int day = pc.GetDayOfMonth(dateTime);
          
          string persianDate = $"{GetPersianDayOfWeek(dateTime)} {day} {GetPersianMonthName(month)} {year}";
          
          if (dateTime.Hour > 0 || dateTime.Minute > 0)
          {
              persianDate += $"، ساعت {dateTime.Hour.ToString("00")}:{dateTime.Minute.ToString("00")}";
          }
          
          return persianDate;
      }
  }
