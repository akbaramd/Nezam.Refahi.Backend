using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Queries.GetCurrentUser;

/// <summary>
/// Query to get the current user's profile
/// </summary>
public record GetCurrentUserQuery : IRequest<ApplicationResult<CurrentUserResponse>>;
