using Swashbuckle.AspNetCore.Filters;

namespace Nezam.Refahi.WebApi.Examples;

public class NationalIdExample : IExamplesProvider<string>
{
  public string GetExamples()
  {
    return "1234567890"; // Valid format Iranian National ID
  }
}