// -----------------------------------------------------------------------------
// Identity.Presentation/Endpoints/ClaimsEndpoints.cs
// -----------------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Identity.Application.Features.Authentication.Queries.GetClaims;
using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Presentation.Endpoints;

public static class ClaimsEndpoints
{
  public static WebApplication MapClaimsEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/v1/claims")
      .WithTags("Claims")
      .RequireAuthorization();

    // ───────────────────── 1) Get All Available Claims ─────────────────────
    group.MapGet("", async (
        [FromServices] IMediator mediator) =>
      {
        var query = new GetClaimsQuery();
        var result = await mediator.Send(query);
        
        if (!result.IsSuccess)
          return Results.BadRequest(result);

        return Results.Ok(result);
      })
      .WithName("GetClaims")
      .WithOpenApi(operation => new(operation)
      {
        Summary = "Get All Available Claims",
        Description = "Returns a list of all distinct claims from registered claim providers.",
        Tags = new List<OpenApiTag> { new() { Name = "Claims" } }
      })
      .Produces<ApplicationResult<IEnumerable<ClaimDto>>>(StatusCodes.Status200OK, "application/json")
      .Produces<ApplicationResult<IEnumerable<ClaimDto>>>(StatusCodes.Status400BadRequest, "application/json");

    return app;
  }
}