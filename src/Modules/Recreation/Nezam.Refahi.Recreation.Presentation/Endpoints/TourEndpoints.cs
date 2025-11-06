// -----------------------------------------------------------------------------
// Recreation.Presentation/Endpoints/TourEndpoints.cs
// -----------------------------------------------------------------------------

using MCA.SharedKernel.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;
using Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetTourDetails;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Recreation.Presentation.Endpoints;

public static class TourEndpoints
{
    public static WebApplication MapTourEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/me/v1/tours")  
            .WithTags("Tours")
            .RequireAuthorization();

        // ───────────────────── 1) Tours - Paginated List ─────────────────────
        group.MapGet("/paginated", async (
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromQuery] string? search,
                [FromQuery] bool? isActive,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUser) =>
            {
                if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
                    return Results.Unauthorized();

                var query = new GetToursPaginatedQuery
                {
                    PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                    PageSize = pageSize <= 0 ? 10 : pageSize,
                    Search = search,
                    IsActive = isActive,
                    ExternalUserId = currentUser.GetRequiredUserId()
                };
                
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetToursPaginated")
            .Produces<ApplicationResult<PaginatedResult<TourWithUserReservationDto>>>()
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Tours Paginated",
                Description = "Returns a paginated list of tours with optional search and filtering.",
                Tags = new List<OpenApiTag> { new() { Name = "Tours" } }
            });

        // ───────────────────── 2) Tours - Details by Id ─────────────────────
        group.MapGet("/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUser) =>
            {
                if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
                    return Results.Unauthorized();

                var result = await mediator.Send(new GetTourDetailsQuery(id, currentUser.GetRequiredUserId()));
            return Results.Ok(result);
        })
        .WithName("GetTourDetails")
        .Produces<ApplicationResult<TourDetailWithUserReservationDto>>();

        // ───────────────────── 3) Me · Tours (user-context enriched) ─────────────────────
        var meGroup = app.MapGroup("/api/me/recreation/tours")
            .WithTags("Me · Tours")
            .RequireAuthorization();

        // 3.1) My tours paginated (adds my reservation summary per tour)
        meGroup.MapGet("/paginated", async (
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] string? search,
            [FromQuery] bool? isActive,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser) =>
        {
            if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
                return Results.Unauthorized();

            var query = new GetToursPaginatedQuery
            {
                PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                PageSize = pageSize <= 0 ? 10 : pageSize,
                Search = search,
                IsActive = isActive,
                ExternalUserId = currentUser.UserId.Value
            };

            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("Me_GetToursPaginated")
        .Produces<ApplicationResult<PaginatedResult<TourWithUserReservationDto>>>();

        // 3.2) My tour details (adds my reservation detail if exists)
        meGroup.MapGet("/{id:guid}", async (
            [FromRoute] Guid id,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser) =>
        {
            if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
                return Results.Unauthorized();

            var result = await mediator.Send(new GetTourDetailsQuery(id, currentUser.UserId.Value));
            return Results.Ok(result);
        })
        .WithName("Me_GetTourDetails")
        .Produces<ApplicationResult<TourDetailWithUserReservationDto>>();

        return app;
    }
}
