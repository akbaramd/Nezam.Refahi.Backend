using Nezam.Refahi.Identity.Presentation.Endpoints;
using Nezam.Refahi.Identity.Presentation.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Nezam.Refahi.WebApi.Examples;

public class CompleteRegistrationRequestExample : IExamplesProvider<CompleteRegistrationRequest>
{
  public CompleteRegistrationRequest GetExamples()
  {
    return new CompleteRegistrationRequest(
      FirstName: "محمد",
      LastName: "احمدی",
      NationalId: "1234567890"
    );
  }
}