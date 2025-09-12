namespace Nezam.Refahi.Identity.Application.Services.Models;

/// <summary>
/// Result of user validation operations
/// </summary>
public class UserValidationResult
{
    /// <summary>
    /// Indicates if the validation was successful
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Validation message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional validation details
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();

    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    /// <param name="message">Success message</param>
    /// <returns>Successful validation result</returns>
    public static UserValidationResult Success(string message = "اعتبارسنجی موفقیت‌آمیز بود")
    {
        return new UserValidationResult
        {
            IsValid = true,
            Message = message
        };
    }

    /// <summary>
    /// Creates a failed validation result
    /// </summary>
    /// <param name="error">Error message</param>
    /// <returns>Failed validation result</returns>
    public static UserValidationResult Failure(string error)
    {
        return new UserValidationResult
        {
            IsValid = false,
            Message = error,
            Errors = new List<string> { error }
        };
    }

    /// <summary>
    /// Creates a failed validation result with multiple errors
    /// </summary>
    /// <param name="errors">List of error messages</param>
    /// <param name="message">General error message</param>
    /// <returns>Failed validation result</returns>
    public static UserValidationResult Failure(IEnumerable<string> errors, string message = "اعتبارسنجی ناموفق بود")
    {
        var errorsList = errors.ToList();
        return new UserValidationResult
        {
            IsValid = false,
            Message = message,
            Errors = errorsList
        };
    }

    /// <summary>
    /// Adds an error to the validation result
    /// </summary>
    /// <param name="error">Error message to add</param>
    public void AddError(string error)
    {
        IsValid = false;
        Errors.Add(error);
    }

    /// <summary>
    /// Adds multiple errors to the validation result
    /// </summary>
    /// <param name="errors">Error messages to add</param>
    public void AddErrors(IEnumerable<string> errors)
    {
        IsValid = false;
        Errors.AddRange(errors);
    }

    /// <summary>
    /// Sets additional validation details
    /// </summary>
    /// <param name="key">Detail key</param>
    /// <param name="value">Detail value</param>
    public void SetDetail(string key, object value)
    {
        Details[key] = value;
    }

    /// <summary>
    /// Gets a detail value by key
    /// </summary>
    /// <param name="key">Detail key</param>
    /// <returns>Detail value or null if not found</returns>
    public T? GetDetail<T>(string key)
    {
        if (Details.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <summary>
    /// Combines multiple validation results
    /// </summary>
    /// <param name="results">Validation results to combine</param>
    /// <returns>Combined validation result</returns>
    public static UserValidationResult Combine(params UserValidationResult[] results)
    {
        var combinedResult = new UserValidationResult
        {
            IsValid = results.All(r => r.IsValid),
            Message = results.All(r => r.IsValid) ? "همه اعتبارسنجی‌ها موفقیت‌آمیز بود" : "برخی اعتبارسنجی‌ها ناموفق بود"
        };

        foreach (var result in results)
        {
            combinedResult.Errors.AddRange(result.Errors);
            foreach (var detail in result.Details)
            {
                combinedResult.Details[detail.Key] = detail.Value;
            }
        }

        return combinedResult;
    }
}