namespace Nezam.Refahi.Identity.Presentation.Models;

public record CompleteRegistrationRequest(
  string FirstName,
  string LastName,
  string NationalId);