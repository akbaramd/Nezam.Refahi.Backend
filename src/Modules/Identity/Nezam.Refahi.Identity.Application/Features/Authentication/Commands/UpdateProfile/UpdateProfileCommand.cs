using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.UpdateProfile;

/// <summary>
/// Command to update user profile information
/// </summary>
public record UpdateProfileCommand : IRequest<ApplicationResult<UpdateProfileResponse>>
{
    /// <summary>
    /// UserDetail's first name
    /// </summary>
    public string FirstName { get; init; } = string.Empty;
    
    /// <summary>
    /// UserDetail's last name
    /// </summary>
    public string LastName { get; init; } = string.Empty;
    
    /// <summary>
    /// UserDetail's national ID
    /// </summary>
    public string NationalId { get; init; } = string.Empty;
    
    /// <summary>
    /// UserDetail's phone number (optional - if provided, will require re-verification)
    /// </summary>
    public string? PhoneNumber { get; init; }
    
    /// <summary>
    /// UserDetail ID (will be populated from JWT token)
    /// </summary>
    public Guid UserId { get; set; }
}