using MediatR;
using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetRoleById;

/// <summary>
/// Query to get a role by its ID
/// </summary>
public class GetRoleByIdQuery : IRequest<ApplicationResult<RoleDto>>
{
    /// <summary>
    /// Role ID to retrieve
    /// </summary>
    public Guid Id { get; set; }
}