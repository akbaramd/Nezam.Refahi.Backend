using MediatR;
using Nezam.Refahi.Application.Common.Models;

namespace Nezam.Refahi.Application.Features.Auth.Queries.GetCurrentUser;

/// <summary>
/// Query to get the current user's profile
/// </summary>
public record GetCurrentUserQuery : IRequest<ApplicationResult<CurrentUserResponse>>;
