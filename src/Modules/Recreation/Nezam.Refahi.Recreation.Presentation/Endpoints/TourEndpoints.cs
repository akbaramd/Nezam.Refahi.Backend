// -----------------------------------------------------------------------------
// Recreation.Presentation/Endpoints/TourEndpoints.cs
// -----------------------------------------------------------------------------

using MCA.SharedKernel.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Presentation.Endpoints;

public static class TourEndpoints
{
    public static WebApplication MapTourEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/tours")
            .WithTags("Tours")
            .RequireAuthorization();

        // ───────────────────── 1) Tours - Paginated List ─────────────────────
        group.MapGet("/paginated", async (
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromQuery] string? search,
                [FromQuery] bool? isActive,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetToursPaginatedQuery
                {
                    PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                    PageSize = pageSize <= 0 ? 10 : pageSize,
                    Search = search,
                    IsActive = isActive,
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetToursPaginated")
            .Produces<ApplicationResult<PaginatedResult<TourDto>>>()
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Tours Paginated",
                Description = "Returns a paginated list of tours with optional search and filtering.",
                Tags = new List<OpenApiTag> { new() { Name = "Tours" } }
            });

        return app;
    }
}
