using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Commands.CompleteRegistration;

/// <summary>
/// Command to complete user registration with additional profile information
/// </summary>
public record CompleteRegistrationCommand : IRequest<ApplicationResult<CompleteRegistrationResponse>>
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; init; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; init; } = string.Empty;
    
    /// <summary>
    /// User's national ID
    /// </summary>
    public string NationalId { get; init; } = string.Empty;
    

    
    /// <summary>
    /// User ID (will be populated from JWT token)
    /// </summary>
    public Guid UserId { get; set; }
}
