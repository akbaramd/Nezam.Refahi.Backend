namespace Nezam.Refahi.Identity.Domain.Services;

/// <summary>
/// Simple validation result class for domain service operations
/// </summary>
public class ValidationResult
{
  private readonly List<string> _errors = new();

  public bool IsValid => !_errors.Any();
  public IReadOnlyList<string> Errors => _errors.AsReadOnly();

  public void AddError(string error)
  {
    if (!string.IsNullOrWhiteSpace(error))
    {
      _errors.Add(error);
    }
  }

  public void AddErrors(IEnumerable<string> errors)
  {
    foreach (var error in errors)
    {
      AddError(error);
    }
  }

  public string GetErrorMessage()
  {
    return string.Join("; ", _errors);
  }
}