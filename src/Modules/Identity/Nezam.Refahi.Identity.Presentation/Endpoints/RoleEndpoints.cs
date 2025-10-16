// -----------------------------------------------------------------------------
// Identity.Presentation/Endpoints/RoleEndpoints.cs
// -----------------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Identity.Application.Features.Roles.Commands.AddClaims;
using Nezam.Refahi.Identity.Application.Features.Roles.Commands.CreateRole;
using Nezam.Refahi.Identity.Application.Features.Roles.Commands.DeleteRole;
using Nezam.Refahi.Identity.Application.Features.Roles.Commands.RemoveClaims;
using Nezam.Refahi.Identity.Application.Features.Roles.Commands.UpdateRole;
using Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetAllRoles;
using Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetRoleById;
using Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetRolesPaginated;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Presentation.Endpoints;

public static class RoleEndpoints
{
    public static WebApplication MapRoleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/roles")
            .WithTags("Roles")
            .RequireAuthorization();

        // ───────────────────── 1) Roles - Get All ─────────────────────
        group.MapGet("", async (
                [FromQuery] bool? isActive,
                [FromQuery] bool? isSystemRole,
                [FromQuery] bool includeClaims,
                [FromQuery] bool includeUserCount,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetAllRolesQuery
                {
                    IsActive = isActive,
                    IsSystemRole = isSystemRole,
                    IncludeClaims = includeClaims,
                    IncludeUserCount = includeUserCount
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAllRoles")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get All Roles",
                Description = "Returns all roles with optional filtering and includes.",
                Tags = new List<OpenApiTag> { new() { Name = "Roles" } }
            });

        // ───────────────────── 2) Roles - Paginated List ─────────────────────
        group.MapGet("/paginated", async (
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromQuery] string? searchTerm,
                [FromQuery] bool? isActive,
                [FromQuery] bool? isSystemRole,
                [FromQuery] string sortBy,
                [FromQuery] string sortDirection,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetRolesPaginatedQuery
                {
                    PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                    PageSize = pageSize <= 0 ? 10 : pageSize,
                    SearchTerm = searchTerm,
                    IsActive = isActive,
                    IsSystemRole = isSystemRole,
                    SortBy = sortBy ?? "DisplayOrder",
                    SortDirection = sortDirection ?? "asc"
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetRolesPaginated")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Roles Paginated",
                Description = "Returns a paginated list of roles with optional search and filtering.",
                Tags = new List<OpenApiTag> { new() { Name = "Roles" } }
            });

        // ───────────────────── 3) Roles - Get By ID ─────────────────────
        group.MapGet("/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetRoleByIdQuery { Id = id };
                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                    return Results.NotFound(result);
                    
                return Results.Ok(result);
            })
            .WithName("GetRoleById")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Role By ID",
                Description = "Returns a role by its unique identifier with claims and user count.",
                Tags = new List<OpenApiTag> { new() { Name = "Roles" } }
            });

        // ───────────────────── 4) Roles - Create ─────────────────────
        group.MapPost("", async (
                [FromBody] CreateRoleRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new CreateRoleCommand
                {
                    Name = request.Name,
                    Description = request.Description,
                    IsSystemRole = request.IsSystemRole,
                    DisplayOrder = request.DisplayOrder
                };

                var result = await mediator.Send(command);
                
                if (!result.IsSuccess)
                    return Results.BadRequest(result);
                    
                return Results.Created($"/api/v1/roles/{result.Data}", result);
            })
            .WithName("CreateRole")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Create Role",
                Description = "Creates a new role in the system.",
                Tags = new List<OpenApiTag> { new() { Name = "Roles" } }
            });

        // ───────────────────── 5) Roles - Update ─────────────────────
        group.MapPut("/{id:guid}", async (
                [FromRoute] Guid id,
                [FromBody] UpdateRoleRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new UpdateRoleCommand
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description,
                    DisplayOrder = request.DisplayOrder
                };

                var result = await mediator.Send(command);
                
                if (!result.IsSuccess)
                    return Results.BadRequest(result);
                    
                return Results.Ok(result);
            })
            .WithName("UpdateRole")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Update Role",
                Description = "Updates an existing role's details.",
                Tags = new List<OpenApiTag> { new() { Name = "Roles" } }
            });

        // ───────────────────── 6) Roles - Delete ─────────────────────
        group.MapDelete("/{id:guid}", async (
                [FromRoute] Guid id,
                [FromQuery] bool forceDelete,
                [FromServices] IMediator mediator) =>
            {
                var command = new DeleteRoleCommand
                {
                    Id = id,
                    ForceDelete = forceDelete
                };

                var result = await mediator.Send(command);
                
                if (!result.IsSuccess)
                    return Results.BadRequest(result);
                    
                return Results.Ok(result);
            })
            .WithName("DeleteRole")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Delete Role",
                Description = "Deletes a role or deactivates it if users are assigned (with forceDelete=true).",
                Tags = new List<OpenApiTag> { new() { Name = "Roles" } }
            });

        // ───────────────────── 7) Roles - Add Claims ─────────────────────
        group.MapPost("/{id:guid}/claims", async (
                [FromRoute] Guid id,
                [FromBody] AddClaimsRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new AddClaimsToRoleCommand
                {
                    RoleId = id,
                    ClaimValues = request.ClaimValues
                };

                var result = await mediator.Send(command);
                
                if (!result.IsSuccess)
                    return Results.BadRequest(result);
                    
                return Results.Ok(result);
            })
            .WithName("AddClaimsToRole")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Add Claims to Role",
                Description = "Adds claims to an existing role.",
                Tags = new List<OpenApiTag> { new() { Name = "Roles" } }
            });

        // ───────────────────── 8) Roles - Remove Claims ─────────────────────
        group.MapDelete("/{id:guid}/claims", async (
                [FromRoute] Guid id,
                [FromBody] RemoveClaimsRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new RemoveClaimsFromRoleCommand
                {
                    RoleId = id,
                    ClaimValues = request.ClaimValues
                };

                var result = await mediator.Send(command);
                
                if (!result.IsSuccess)
                    return Results.BadRequest(result);
                    
                return Results.Ok(result);
            })
            .WithName("RemoveClaimsFromRole")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Remove Claims from Role",
                Description = "Removes claims from an existing role.",
                Tags = new List<OpenApiTag> { new() { Name = "Roles" } }
            });

        return app;
    }
}

// ═══════════════════════ REQUEST/RESPONSE DTOs ══════════════════════════════

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}

public class AddClaimsRequest
{
    public List<string> ClaimValues { get; set; } = new();
}

public class RemoveClaimsRequest
{
    public List<string> ClaimValues { get; set; } = new();
}