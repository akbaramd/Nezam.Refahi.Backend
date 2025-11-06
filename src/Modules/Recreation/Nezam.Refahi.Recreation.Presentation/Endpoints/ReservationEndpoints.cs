using MCA.SharedKernel.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.AddGuest;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ChangeReservationCapacity;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.FinalizeReservation;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.RemoveGuest;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.StartReservation;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ReactivateReservation;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationDetail;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationPricing;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationsPaginated;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Presentation.Endpoints;

public static class MeReservationEndpoints
{
    public static void MapMeReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/me/recreation/reservations")
            .WithTags("Tour Reservations")
            .RequireAuthorization();

        // 0) Get paginated list of reservations
        group.MapGet("/paginated", async (
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] ReservationStatus? status,
            [FromQuery] string? search,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
                return Results.Unauthorized();

            var query = new GetReservationsPaginatedQuery
            {
                PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                PageSize = pageSize <= 0 ? 10 : pageSize,
                Status = status,
                Search = search,
                FromDate = fromDate,
                ToDate = toDate,
                ExternalUserId = currentUser.UserId.Value
            };

            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("Me_GetReservationsPaginated")
        .WithSummary("Get paginated list of reservations")
        .WithDescription("Returns a paginated list of reservations for the current user with optional filtering by status, search term, and date range.")
        .Produces<ApplicationResult<PaginatedResult<ReservationDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

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

        // 2.6) Reactivate expired reservation to Draft
        group.MapPost("/{reservationId:guid}/reactivate", async (
            [FromRoute] Guid reservationId,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.UserId is null)
                return Results.Unauthorized();

            var cmd = new ReactivateReservationCommand
            {
                ReservationId = reservationId,
                ExternalUserId = currentUser.UserId.Value,
                Reason = "درخواست کاربر برای بازگشت به پیش‌نویس"
            };

            var result = await mediator.Send(cmd, ct);
            return Results.Ok(result);
        })
        .WithName("Me_ReactivateReservation")
        .WithSummary("Reactivate expired reservation to Draft")
        .WithDescription("Returns an expired reservation to Draft, clears expiry date and price snapshots. Ownership is enforced in the handler.")
        .Produces<ApplicationResult<ReactivateReservationResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // 2.5) Change reservation capacity
        group.MapPut("/{reservationId:guid}/capacity", async (
            [FromRoute] Guid reservationId,
            [FromBody] ChangeReservationCapacityRequest body,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.UserId is null)
                return Results.Unauthorized();

            var cmd = new ChangeReservationCapacityCommand
            {
                ReservationId = reservationId,
                NewCapacityId = body.NewCapacityId,
                ExternalUserId = currentUser.UserId.Value
            };

            var result = await mediator.Send(cmd, ct);
            return Results.Ok(result);
        })
        .WithName("Me_ChangeReservationCapacity")
        .WithSummary("Change reservation capacity")
        .WithDescription("Changes the capacity of a draft reservation. Ownership is enforced in the handler. Only draft reservations can have their capacity changed.")
        .Produces<ApplicationResult<ChangeReservationCapacityCommandResult>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

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

        // 4) Finalize reservation
        group.MapPost("/{reservationId:guid}/finalize", async (
            [FromRoute] Guid reservationId,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            if (currentUser.UserId is null)
                return Results.Unauthorized();

            var cmd = new FinalizeReservationCommand(reservationId);

            var result = await mediator.Send(cmd, ct);
            return Results.Ok(result);
        })
        .WithName("Me_FinalizeReservation")
        .WithSummary("Finalize reservation")
        .WithDescription("Finalizes a draft reservation by validating participants, creating pricing snapshots, creating a bill, and transitioning to OnHold status. Ownership is enforced in the handler.")
        .Produces<ApplicationResult<FinalizeReservationResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        // 5) Get reservation pricing (pricing-only)
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

        // 6) Get reservation detail (read model)
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

// Request model for Change Capacity
public sealed record ChangeReservationCapacityRequest
{
    public Guid NewCapacityId { get; init; }
}
