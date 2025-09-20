using Swashbuckle.AspNetCore.Filters;
using Nezam.Refahi.Identity.Presentation.Endpoints;
using Nezam.Refahi.Identity.Presentation.Models;

namespace Nezam.Refahi.WebApi.Examples;

public class SendOtpRequestExample : IExamplesProvider<SendOtpRequest>
{
    public SendOtpRequest GetExamples()
    {
        return new SendOtpRequest(
            NationalCode: "1234567890",
            Purpose: "login",
            DeviceId: "mobile_app_v1.0",
            Scope: "app"
        );
    }
}