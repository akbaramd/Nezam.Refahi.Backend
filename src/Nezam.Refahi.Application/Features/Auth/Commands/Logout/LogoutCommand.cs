using MediatR;

namespace Nezam.Refahi.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Command to log out a user
/// </summary>
public record LogoutCommand : IRequest<LogoutResponse>
{
    /// <summary>
    /// The user ID to log out
    /// </summary>
    public Guid UserId { get; init; }
}
