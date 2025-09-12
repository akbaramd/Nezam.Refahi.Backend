using Swashbuckle.AspNetCore.Filters;

namespace Nezam.Refahi.WebApi.Examples;

public class PhoneNumberExample : IExamplesProvider<string>
{
  public string GetExamples()
  {
    return "09123456789"; // Valid Iranian mobile number format
  }
}