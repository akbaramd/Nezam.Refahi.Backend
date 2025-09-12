using Nezam.Refahi.Identity.Presentation.Endpoints;
using Nezam.Refahi.Identity.Presentation.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Nezam.Refahi.WebApi.Examples;

public class VerifyOtpRequestExample : IExamplesProvider<VerifyOtpRequest>
{
  public VerifyOtpRequest GetExamples()
  {
    return new VerifyOtpRequest(
      ChallengeId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      OtpCode: "123456",
      Purpose: "login",
      DeviceId: "mobile_app_v1.0",
      Scope: "app"
    );
  }
}