using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.Logout;

/// <summary>
/// Command to log out a user
/// </summary>
public record LogoutCommand : IRequest<ApplicationResult>
{
  public LogoutCommand(Guid userId, string accessToken, string? refreshToken)
  {
    UserId = userId;
    RefreshToken = refreshToken;
    AccessToken = accessToken;
  }

  /// <summary>
    /// The user ID to log out
    /// </summary>
    public Guid UserId { get; init; }

    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
}
