using Nezam.Refahi.Identity.Presentation.Endpoints;
using Nezam.Refahi.Identity.Presentation.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Nezam.Refahi.WebApi.Examples;

public class LogoutRequestExample : IExamplesProvider<LogoutRequest>
{
  public LogoutRequest GetExamples()
  {
    return new LogoutRequest(
      RefreshToken: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.refresh_token_payload.signature"
    );
  }
}