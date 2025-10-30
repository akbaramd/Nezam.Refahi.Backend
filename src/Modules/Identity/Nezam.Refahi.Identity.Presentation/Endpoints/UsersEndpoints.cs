// -----------------------------------------------------------------------------
// Identity.Presentation/Endpoints/UsersEndpoints.cs
// -----------------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Identity.Application.Features.Users.Commands.AddClaims;
using Nezam.Refahi.Identity.Application.Features.Users.Commands.AddRole;
using Nezam.Refahi.Identity.Application.Features.Users.Commands.CreateUser;
using Nezam.Refahi.Identity.Application.Features.Users.Commands.DeleteClaims;
using Nezam.Refahi.Identity.Application.Features.Users.Commands.DeleteUser;
using Nezam.Refahi.Identity.Application.Features.Users.Commands.RemoveRole;
using Nezam.Refahi.Identity.Application.Features.Users.Commands.UpdateUser;
using Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUserDetail;
using Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUsersPaginated;
using Nezam.Refahi.Identity.Domain.Dtos;
using MCA.SharedKernel.Application.Contracts;
using MCA.SharedKernel.Domain.Models;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Presentation.Endpoints;

public static class UsersEndpoints
{
  public static WebApplication MapUsersEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/v1/users")
      .WithTags("Users")
      .RequireAuthorization();

    // ───────────────────── 0) Users - Paginated List ─────────────────────
    group.MapGet("", async (
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromServices] IMediator mediator) =>
      {
        var query = new GetUsersPaginatedQuery
        {
          PageNumber = pageNumber <= 0 ? 1 : pageNumber,
          PageSize   = pageSize   <= 0 ? 20 : pageSize,
          Search     = search
        };

        var result = await mediator.Send(query);
        return Results.Ok(result);
      })
      .WithName("GetUsersPaginated")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Get Users Paginated",
        Description = "Returns a paginated list of users with optional search filter.",
        Tags = new List<OpenApiTag> { new() { Name = "Users" } }
      })
      .Produces<ApplicationResult<PaginatedResult<UserDto>>>(StatusCodes.Status200OK, "application/json");

    
    // ───────────────────── 1) UserDetail Detail ─────────────────────
    group.MapGet("/{id:guid}", async (
        Guid id,
        [FromServices] IMediator mediator) =>
      {
        var result = await mediator.Send(new GetUserDetailQuery(id));
        if (!result.IsSuccess || result.Data is null)
          return Results.NotFound(ApplicationResult<UserDetailDto>.Failure("UserDetail not found."));

        return Results.Ok(ApplicationResult<UserDetailDto>.Success(result.Data.UserDetail));
      })
      .WithName("GetUserDetail")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Get UserDetail Detail",
        Description = "Returns full UserDetail DTO with roles, claims, preferences and tokens.",
        Tags = new List<OpenApiTag> { new() { Name = "Users" } }
      })
      .Produces<ApplicationResult<UserDetailDto>>(StatusCodes.Status200OK, "application/json")
      .Produces<ApplicationResult<UserDetailDto>>(StatusCodes.Status404NotFound, "application/json");

    
    // ───────────────────── 2) Create User ─────────────────────
    group.MapPost("", async (
        [FromBody] CreateUserCommand command,
        [FromServices] IMediator mediator) =>
      {
        var result = await mediator.Send(command);
        if (!result.IsSuccess)
          return Results.BadRequest(result);

        return Results.Created($"/api/v1/users/{result.Data}", ApplicationResult<Guid>.Success(result.Data));
      })
      .WithName("CreateUser")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Create User",
        Description = "Creates a new user with the provided information.",
        Tags = new List<OpenApiTag> { new() { Name = "Users" } }
      })
      .Produces<ApplicationResult<Guid>>(StatusCodes.Status201Created, "application/json")
      .Produces<ApplicationResult<Guid>>(StatusCodes.Status400BadRequest, "application/json");

    
    // ───────────────────── 3) Update User ─────────────────────
    group.MapPut("/{id:guid}", async (
        Guid id,
        [FromBody] UpdateUserCommand command,
        [FromServices] IMediator mediator) =>
      {
        command.Id = id;
        var result = await mediator.Send(command);
        
        if (!result.IsSuccess)
          return Results.BadRequest(result);

        return Results.Ok(result);
      })
      .WithName("UpdateUser")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Update User",
        Description = "Updates an existing user with the provided information.",
        Tags = new List<OpenApiTag> { new() { Name = "Users" } }
      })
      .Produces<ApplicationResult>(StatusCodes.Status200OK, "application/json")
      .Produces<ApplicationResult>(StatusCodes.Status400BadRequest, "application/json");

    
    // ───────────────────── 4) Delete User ─────────────────────
    group.MapDelete("/{id:guid}", async (
        Guid id,
        [FromQuery] bool softDelete ,
        [FromQuery] string? deleteReason ,
        [FromServices] IMediator mediator) =>
      {
        var command = new DeleteUserCommand 
        { 
          Id = id, 
          SoftDelete = softDelete,
          DeleteReason = deleteReason
        };
        
        var result = await mediator.Send(command);
        
        if (!result.IsSuccess)
          return Results.BadRequest(result);

        return Results.Ok(result);
      })
      .WithName("DeleteUser")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Delete User",
        Description = "Deletes a user. By default performs soft delete, but can perform hard delete if specified.",
        Tags = new List<OpenApiTag> { new() { Name = "Users" } }
      })
      .Produces<ApplicationResult>(StatusCodes.Status200OK, "application/json")
      .Produces<ApplicationResult>(StatusCodes.Status400BadRequest, "application/json");

    
    // ───────────────────── 5) Add Claims to User ─────────────────────
    group.MapPost("/{id:guid}/claims", async (
        Guid id,
        [FromBody] AddClaimsToUserCommand command,
        [FromServices] IMediator mediator) =>
      {
        command.UserId = id;
        var result = await mediator.Send(command);
        
        if (!result.IsSuccess)
          return Results.BadRequest(result);

        return Results.Ok(result);
      })
      .WithName("AddClaimsToUser")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Add Claims to User",
        Description = "Adds the specified claims to a user. Claims are validated against available claim providers.",
        Tags = new List<OpenApiTag> { new() { Name = "Users" } }
      })
      .Produces<ApplicationResult>(StatusCodes.Status200OK, "application/json")
      .Produces<ApplicationResult>(StatusCodes.Status400BadRequest, "application/json");

    
    // ───────────────────── 6) Delete Claims from User ─────────────────────
    group.MapDelete("/{id:guid}/claims", async (
        Guid id,
        [FromBody] DeleteClaimsFromUserCommand command,
        [FromServices] IMediator mediator) =>
      {
        command.UserId = id;
        var result = await mediator.Send(command);
        
        if (!result.IsSuccess)
          return Results.BadRequest(result);

        return Results.Ok(result);
      })
      .WithName("DeleteClaimsFromUser")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Delete Claims from User",
        Description = "Removes the specified claims from a user. Claims are soft-deleted by deactivating them.",
        Tags = new List<OpenApiTag> { new() { Name = "Users" } }
      })
      .Produces<ApplicationResult>(StatusCodes.Status200OK, "application/json")
      .Produces<ApplicationResult>(StatusCodes.Status400BadRequest, "application/json");

    
    // ───────────────────── 7) Add Role to User ─────────────────────
    group.MapPost("/{id:guid}/roles", async (
        Guid id,
        [FromBody] AddRoleToUserRequest request,
        [FromServices] IMediator mediator) =>
      {
        var command = new AddRoleToUserCommand
        {
          UserId = id,
          RoleId = request.RoleId,
          ExpiresAt = request.ExpiresAt,
          AssignedBy = request.AssignedBy,
          Notes = request.Notes
        };
        
        var result = await mediator.Send(command);
        
        if (!result.IsSuccess)
          return Results.BadRequest(result);

        return Results.Ok(result);
      })
      .WithName("AddRoleToUser")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Add Role to User",
        Description = "Assigns a role to a user with optional expiration and audit information.",
        Tags = new List<OpenApiTag> { new() { Name = "Users" } }
      })
      .Produces<ApplicationResult>(StatusCodes.Status200OK, "application/json")
      .Produces<ApplicationResult>(StatusCodes.Status400BadRequest, "application/json");

    
    // ───────────────────── 8) Remove Role from User ─────────────────────
    group.MapDelete("/{id:guid}/roles/{roleId:guid}", async (
        Guid id,
        Guid roleId,
        [FromQuery] Guid? removedBy,
        [FromQuery] string? reason,
        [FromServices] IMediator mediator) =>
      {
        var command = new RemoveRoleFromUserCommand
        {
          UserId = id,
          RoleId = roleId,
          RemovedBy = removedBy,
          Reason = reason
        };
        
        var result = await mediator.Send(command);
        
        if (!result.IsSuccess)
          return Results.BadRequest(result);

        return Results.Ok(result);
      })
      .WithName("RemoveRoleFromUser")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Remove Role from User",
        Description = "Removes a role assignment from a user with optional audit information.",
        Tags = new List<OpenApiTag> { new() { Name = "Users" } }
      })
      .Produces<ApplicationResult>(StatusCodes.Status200OK, "application/json")
      .Produces<ApplicationResult>(StatusCodes.Status400BadRequest, "application/json");

    return app;
  }
}

// ═══════════════════════ REQUEST DTOs ══════════════════════════════

public class AddRoleToUserRequest
{
  public Guid RoleId { get; set; }
  public DateTime? ExpiresAt { get; set; }
  public Guid? AssignedBy { get; set; }
  public string? Notes { get; set; }
}
