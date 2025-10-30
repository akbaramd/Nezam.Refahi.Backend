namespace Nezam.Refahi.Shared.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
public class AuthenticationException : ApplicationException
{
    public AuthenticationException(string message)
        : base("Authentication Error", message)
    {
    }
}
