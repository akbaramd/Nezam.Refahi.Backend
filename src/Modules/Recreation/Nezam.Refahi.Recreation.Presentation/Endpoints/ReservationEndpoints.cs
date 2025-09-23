using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.AddGuest;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CreateReservation;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.InitiatePayment;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CancelReservation;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ConfirmReservation;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ReactivateExpiredReservation;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationPricing;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationDetail;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetUserReservations;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Presentation.Endpoints;

public static class ReservationEndpoints
{
    public static void MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/recreation/reservations")
            .WithTags("Tour Reservations")
            .RequireAuthorization();

        // Create reservation
        group.MapPost("/", async (
            [FromBody] CreateTourReservationCommand command,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("CreateTourReservation")
        .WithSummary("Create a new tour reservation")
        .WithDescription("Creates a new reservation for a tour with the authenticated member as main participant")
        .Produces<ApplicationResult<CreateTourReservationResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        // Add guest to reservation
        group.MapPost("/{reservationId:guid}/guests", async (
            [FromRoute] Guid reservationId,
            [FromBody] GuestParticipantDto guest,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new AddGuestToReservationCommand
            {
                ReservationId = reservationId,
                Guest = guest
            };

            var result = await mediator.Send(command, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("AddGuestToReservation")
        .WithSummary("Add guest to existing reservation")
        .WithDescription("Adds a guest participant to an existing pending reservation")
        .Produces<ApplicationResult<AddGuestToReservationResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // Get reservation pricing
        group.MapGet("/{identifier}/pricing", async (
            [FromRoute] string identifier,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetReservationPricingQuery
            {
                ReservationIdentifier = identifier
            };

            var result = await mediator.Send(query, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("GetReservationPricing")
        .WithSummary("Get reservation pricing details")
        .WithDescription("Gets detailed pricing information for a reservation by ID or tracking code")
        .Produces<ReservationPricingResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // Get reservation by tracking code (public endpoint for payment)
        group.MapGet("/track/{trackingCode}", async (
            [FromRoute] string trackingCode,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetReservationPricingQuery
            {
                ReservationIdentifier = trackingCode
            };

            var result = await mediator.Send(query, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("TrackReservation")
        .WithSummary("Track reservation by code")
        .WithDescription("Gets reservation details by tracking code - used for payment processing")
        .Produces<ReservationPricingResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .AllowAnonymous(); // Allow anonymous access for payment tracking

        // Get reservation detail
        group.MapGet("/{reservationId:guid}", async (
            [FromRoute] Guid reservationId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetReservationDetailQuery(reservationId);
            var result = await mediator.Send(query, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("GetReservationDetail")
        .WithSummary("Get reservation details")
        .WithDescription("Gets detailed information about a specific reservation")
        .Produces<ApplicationResult<ReservationDetailDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // Initiate payment for reservation
        group.MapPost("/{reservationId:guid}/payment", async (
            [FromRoute] Guid reservationId,
            [FromBody] InitiatePaymentRequest? request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new InitiatePaymentCommand
            {
                ReservationId = reservationId,
                PaymentMethod = request?.PaymentMethod
            };

            var result = await mediator.Send(command, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("InitiatePayment")
        .WithSummary("Initiate payment for reservation")
        .WithDescription("Initiates payment process for a confirmed reservation")
        .Produces<ApplicationResult<InitiatePaymentResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // Confirm reservation payment
        group.MapPost("/{reservationId:guid}/confirm", async (
            [FromRoute] Guid reservationId,
            [FromBody] ConfirmReservationRequest? request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new ConfirmReservationCommand(
                reservationId,
                request?.TotalAmountRials,
                request?.PaymentReference);

            var result = await mediator.Send(command, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("ConfirmReservation")
        .WithSummary("Confirm reservation payment")
        .WithDescription("Confirms a reservation after successful payment")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // Cancel reservation
        group.MapDelete("/{reservationId:guid}", async (
            [FromRoute] Guid reservationId,
            [FromBody] CancelReservationRequest? request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new CancelReservationCommand(reservationId, request?.Reason, request?.PermanentDelete ?? true);
            var result = await mediator.Send(command, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("CancelReservation")
        .WithSummary("Cancel reservation")
        .WithDescription("Cancels an existing reservation with optional reason. Can permanently delete or just mark as cancelled.")
        .Produces<ApplicationResult<CancelReservationResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // Reactivate expired reservation
        group.MapPost("/{reservationId:guid}/reactivate", async (
            [FromRoute] Guid reservationId,
            [FromBody] ReactivateReservationRequest? request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new ReactivateExpiredReservationCommand(
                reservationId, 
                request?.Reason);
            
            var result = await mediator.Send(command, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("ReactivateExpiredReservation")
        .WithSummary("Reactivate expired reservation")
        .WithDescription("Attempts to reactivate an expired reservation if capacity is available. If no capacity, the reservation will be deleted.")
        .Produces<ApplicationResult<ReactivateExpiredReservationResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        // Get current user's reservations
        group.MapGet("/my-reservations", async (
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ReservationStatus? status = null,
            [FromQuery] string? trackingCode = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null) =>
        {
            var query = new GetUserReservationsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Status = status,
                TrackingCode = trackingCode,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await mediator.Send(query, cancellationToken:cancellationToken);
            return result;
        })
        .WithName("GetUserReservations")
        .WithSummary("Get current user's reservations")
        .WithDescription("Gets paginated list of reservations for the current authenticated user, including participant information")
        .Produces<ApplicationResult<PaginatedResult<UserReservationDto>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}

// Request DTOs for endpoint parameters
public record InitiatePaymentRequest
{
    public string? PaymentMethod { get; init; }
}

public record ConfirmReservationRequest
{
    public long? TotalAmountRials { get; init; }
    public string? PaymentReference { get; init; }
}

public record CancelReservationRequest
{
    public string? Reason { get; init; }
    public bool? PermanentDelete { get; init; }
}

public record ReactivateReservationRequest
{
    public string? Reason { get; init; }
    
}