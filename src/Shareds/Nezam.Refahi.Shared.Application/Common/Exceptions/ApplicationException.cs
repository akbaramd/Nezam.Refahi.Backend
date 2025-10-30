namespace Nezam.Refahi.Shared.Application.Common.Exceptions;

/// <summary>
/// Base exception for all application-specific exceptions
/// </summary>
public abstract class ApplicationException : Exception
{
    protected ApplicationException(string title, string message) 
        : base(message)
    {
        Title = title;
    }

    public string Title { get; }
}
