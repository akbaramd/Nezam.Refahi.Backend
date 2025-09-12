namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.Logout;

/// <summary>
/// Response for the LogoutCommand
/// </summary>
public class LogoutResponse
{
    /// <summary>
    /// Whether the logout was successful
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// The message to display to the user
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
