using MCA.SharedKernel.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.AddGuest;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.RemoveGuest;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.StartReservation;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationDetail;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationPricing;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Presentation.Endpoints;

public static class MeReservationEndpoints
{
    public static void MapMeReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/me/recreation/reservations")
            .WithTags("Me · Tour Reservations")
            .RequireAuthorization();

        // 1) Start reservation (Draft)
        group.MapPost("/start", async (
            [FromBody] StartReservationRequest body,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(currentUser.UserNationalNumber))
                return Results.Unauthorized();

            var cmd = new StartReservationCommand
            {
                TourId = body.TourId,
                CapacityId = body.CapacityId,             // optional
                UserNationalNumber = currentUser.UserNationalNumber,
            };

            var result = await mediator.Send(cmd, ct);
            return Results.Ok(result);
        })
        .WithName("Me_StartReservation")
        .WithSummary("Start reservation (Draft)")
        .WithDescription("Creates a Draft reservation for the current user. CapacityId is optional.")
        .Produces<ApplicationResult<StartReservationCommandResult>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest);

        // 2) Add guest
        group.MapPost("/{reservationId:guid}/guests", async (
            [FromRoute] Guid reservationId,
            [FromBody] GuestParticipantDto guest,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.UserId is null)
                return Results.Unauthorized();

            var cmd = new AddGuestToReservationCommand
            {
                ReservationId = reservationId,
                ExternalUserId = currentUser.UserId.Value,
                Guest = guest
            };

            var result = await mediator.Send(cmd, ct);
            return Results.Ok(result);
        })
        .WithName("Me_AddGuestToReservation")
        .WithSummary("Add guest to reservation")
        .WithDescription("Adds a guest to the current user's reservation. Ownership is enforced in the handler.")
        .Produces<ApplicationResult<AddGuestToReservationResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // 3) Remove guest
        group.MapDelete("/{reservationId:guid}/guests/{participantId:guid}", async (
            [FromRoute] Guid reservationId,
            [FromRoute] Guid participantId,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.UserId is null)
                return Results.Unauthorized();

            var cmd = new RemoveGuestFromReservationCommand
            {
                ReservationId = reservationId,
                ParticipantId = participantId,
                ExternalUserId = currentUser.UserId.Value
            };

            var result = await mediator.Send(cmd, ct);
            return Results.Ok(result);
        })
        .WithName("Me_RemoveGuestFromReservation")
        .WithSummary("Remove guest from reservation")
        .WithDescription("Removes a guest from the current user's reservation. Handler enforces ownership.")
        .Produces<ApplicationResult<RemoveGuestFromReservationResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // 4) Get reservation pricing (pricing-only)
        group.MapGet("/{reservationId:guid}/pricing", async (
            [FromRoute] Guid reservationId,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.UserId is null)
                return Results.Unauthorized();

            var query = new GetReservationPricingQuery
            {
                ReservationId = reservationId,
            };

            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("Me_GetReservationPricing")
        .WithSummary("Get reservation pricing")
        .WithDescription("Returns totals and per-participant pricing for the current user's reservation.")
        .Produces<ApplicationResult<ReservationPricingResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // 5) Get reservation detail (read model)
        group.MapGet("/{reservationId:guid}", async (
            [FromRoute] Guid reservationId,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.UserId is null)
                return Results.Unauthorized();

            var query = new GetReservationDetailQuery(reservationId,currentUser.UserNationalNumber!); // overload that enforces ownership
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("Me_GetReservationDetail")
        .WithSummary("Get reservation detail")
        .WithDescription("Returns reservation details for the current user. Handler must ensure the reservation belongs to the user.")
        .Produces<ApplicationResult<ReservationDetailDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }
}

// Request model for Start
public sealed record StartReservationRequest
{
    public Guid TourId { get; init; }
    public Guid CapacityId { get; init; } // optional
}
