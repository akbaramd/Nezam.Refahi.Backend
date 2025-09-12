using Swashbuckle.AspNetCore.Filters;

namespace Nezam.Refahi.WebApi.Examples;

public class ErrorResponseExample : IExamplesProvider<object>
{
  public object GetExamples()
  {
    return new
    {
      success = false,
      error = new
      {
        code = "VALIDATION_ERROR",
        message = "داده‌های ارسالی نامعتبر است",
        details = new[]
        {
          "نام کاربری الزامی است",
          "رمز عبور باید حداقل ۸ کاراکتر باشد"
        }
      },
      timestamp = DateTime.UtcNow
    };
  }
}