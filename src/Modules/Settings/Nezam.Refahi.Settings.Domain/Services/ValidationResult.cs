namespace Nezam.Refahi.Settings.Domain.Services;

/// <summary>
/// Result of a validation operation
/// </summary>
public class ValidationResult
{
  public bool IsValid { get; }
  public List<string> Errors { get; }
  public List<string> Warnings { get; }

  public ValidationResult(bool isValid = true)
  {
    IsValid = isValid;
    Errors = new List<string>();
    Warnings = new List<string>();
  }

  public ValidationResult AddError(string error)
  {
    Errors.Add(error);
    return this;
  }

  public ValidationResult AddWarning(string warning)
  {
    Warnings.Add(warning);
    return this;
  }

  public ValidationResult AddErrors(IEnumerable<string> errors)
  {
    Errors.AddRange(errors);
    return this;
  }

  public ValidationResult AddWarnings(IEnumerable<string> warnings)
  {
    Warnings.AddRange(warnings);
    return this;
  }

  public static ValidationResult Success() => new ValidationResult(true);
  public static ValidationResult Failure(params string[] errors) => new ValidationResult(false).AddErrors(errors);
}