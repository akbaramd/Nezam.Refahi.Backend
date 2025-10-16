using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;

namespace Nezam.Refahi.BasicDefinitions.Presentation.DTOs;

/// <summary>
/// Response DTO for Agency API operations
/// </summary>
public sealed class AgencyResponse
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The office data (null if operation failed)
    /// </summary>
    public AgencyDto? Data { get; set; }

    /// <summary>
    /// List of validation errors (if any)
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static AgencyResponse SuccessResult(AgencyDto data, string message = "Operation completed successfully")
    {
        return new AgencyResponse
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = new List<string>()
        };
    }

    /// <summary>
    /// Creates a failed response
    /// </summary>
    public static AgencyResponse FailureResult(string message, List<string>? errors = null)
    {
        return new AgencyResponse
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = errors ?? new List<string>()
        };
    }
}
