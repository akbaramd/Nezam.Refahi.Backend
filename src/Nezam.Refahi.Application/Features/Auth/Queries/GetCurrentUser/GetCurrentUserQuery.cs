using MediatR;

namespace Nezam.Refahi.Application.Features.Auth.Queries.GetCurrentUser;

/// <summary>
/// Query to get the current user's profile
/// </summary>
public record GetCurrentUserQuery : IRequest<CurrentUserResponse>;
