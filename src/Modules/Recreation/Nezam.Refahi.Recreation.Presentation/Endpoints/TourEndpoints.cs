// -----------------------------------------------------------------------------
// Recreation.Presentation/Endpoints/TourEndpoints.cs
// -----------------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.AddGuest;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CreateReservation;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationPricing;
using Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;
using Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetTourDetail;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
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

        // ───────────────────── 2) Tours - Get All ─────────────────────
        group.MapGet("", async (
                [FromQuery] bool? isActive,
                [FromQuery] bool? registrationOpen,
                [FromServices] IMediator mediator) =>
            {
                // For now, we'll use the paginated query with a large page size
                // Later, you might want to create a dedicated GetAllToursQuery
                var query = new GetToursPaginatedQuery
                {
                    PageNumber = 1,
                    PageSize = 1000, // Large enough to get all tours
                    IsActive = isActive
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAllTours")
            .Produces<ApplicationResult<PaginatedResult<TourDto>>>()
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get All Tours",
                Description = "Returns all tours with optional filtering.",
                Tags = new List<OpenApiTag> { new() { Name = "Tours" } }
            });
        
        // ───────────────────── 3) Tours - Get Tour Detail ─────────────────────
        group.MapGet("/{tourId:guid}", async (
                [FromRoute] Guid tourId,
                [FromQuery] bool includeUserInfo,
                [FromQuery] bool includeStatistics,
                [FromQuery] bool includeCapacityDetails,
                [FromQuery] bool includePricing,
                [FromQuery] bool includeMedia,
                [FromQuery] bool includeFeatures,
                [FromQuery] bool includeRestrictions,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetTourDetailQuery
                {
                    TourId = tourId,
                    IncludeUserInfo = includeUserInfo,
                    IncludeStatistics = includeStatistics,
                    IncludeCapacityDetails = includeCapacityDetails,
                    IncludePricing = includePricing,
                    IncludeMedia = includeMedia,
                    IncludeFeatures = includeFeatures,
                    IncludeRestrictions = includeRestrictions
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok(result);
            })
            .WithName("GetTourDetail")
            .Produces<ApplicationResult<TourDetailDto>>()
            .Produces(400)
            .Produces(404)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Tour Detail",
                Description = "Returns detailed information about a specific tour including capacity, pricing, features, and user-specific data.",
                Tags = new List<OpenApiTag> { new() { Name = "Tours" } }
            });

        // ───────────────────── 4) Tours - Get Tour Summary (Lightweight) ─────────────────────
        group.MapGet("/{tourId:guid}/summary", async (
                [FromRoute] Guid tourId,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetTourDetailQuery
                {
                    TourId = tourId,
                    IncludeUserInfo = false,
                    IncludeStatistics = false,
                    IncludeCapacityDetails = true,
                    IncludePricing = true,
                    IncludeMedia = false,
                    IncludeFeatures = false,
                    IncludeRestrictions = false
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok(result);
            })
            .WithName("GetTourSummary")
            .Produces<ApplicationResult<TourDetailDto>>()
            .Produces(400)
            .Produces(404)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Tour Summary",
                Description = "Returns basic tour information with capacity and pricing details (lightweight version).",
                Tags = new List<OpenApiTag> { new() { Name = "Tours" } }
            });

        // ───────────────────── 5) Tours - Get Tour Capacity ─────────────────────
        group.MapGet("/{tourId:guid}/capacity", async (
                [FromRoute] Guid tourId,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetTourDetailQuery
                {
                    TourId = tourId,
                    IncludeUserInfo = false,
                    IncludeStatistics = false,
                    IncludeCapacityDetails = true,
                    IncludePricing = false,
                    IncludeMedia = false,
                    IncludeFeatures = false,
                    IncludeRestrictions = false
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                // Return only capacity-related information
                var capacityInfo = new TourCapacityResponse
                {
                    TourId = result.Data?.Id ?? Guid.Empty,
                    Title = result.Data?.Title ?? string.Empty,
                    Capacities = result.Data?.Capacities ?? new List<TourCapacityDetailDto>(),
                    MaxCapacity = result.Data?.MaxCapacity ?? 0,
                    RemainingCapacity = result.Data?.RemainingCapacity ?? 0,
                    ReservedCapacity = result.Data?.ReservedCapacity ?? 0,
                    UtilizationPercentage = result.Data?.CapacityUtilizationPercentage ?? 0,
                    IsFullyBooked = result.Data?.IsFullyBooked ?? false,
                    IsNearlyFull = result.Data?.IsNearlyFull ?? false,
                    CapacityStatus = result.Data?.CapacityStatus ?? string.Empty,
                    CapacityMessage = result.Data?.CapacityMessage ?? string.Empty,
                    IsRegistrationOpen = result.Data?.IsRegistrationOpen ?? false,
                    RegistrationEnd = result.Data?.RegistrationEnd
                };
                
                return Results.Ok(capacityInfo);
            })
            .WithName("GetTourCapacity")
            .Produces<TourCapacityResponse>(200)
            .Produces(400)
            .Produces(404)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Tour Capacity",
                Description = "Returns detailed capacity information for a specific tour.",
                Tags = new List<OpenApiTag> { new() { Name = "Tours" } }
            });

        // ───────────────────── 6) Tours - Get Tour Pricing ─────────────────────
        group.MapGet("/{tourId:guid}/pricing", async (
                [FromRoute] Guid tourId,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetTourDetailQuery
                {
                    TourId = tourId,
                    IncludeUserInfo = false,
                    IncludeStatistics = false,
                    IncludeCapacityDetails = false,
                    IncludePricing = true,
                    IncludeMedia = false,
                    IncludeFeatures = false,
                    IncludeRestrictions = false
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                // Return only pricing-related information
                var pricingInfo = new TourPricingResponse
                {
                    TourId = result.Data?.Id ?? Guid.Empty,
                    Title = result.Data?.Title ?? string.Empty,
                    Pricing = result.Data?.Pricing ?? new List<TourPricingDetailDto>(),
                    LowestPrice = result.Data?.LowestPrice,
                    HighestPrice = result.Data?.HighestPrice,
                    HasDiscount = result.Data?.HasDiscount ?? false,
                    MaxDiscountPercentage = result.Data?.MaxDiscountPercentage
                };
                
                return Results.Ok(pricingInfo);
            })
            .WithName("GetTourPricing")
            .Produces<TourPricingResponse>(200)
            .Produces(400)
            .Produces(404)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Tour Pricing",
                Description = "Returns detailed pricing information for a specific tour.",
                Tags = new List<OpenApiTag> { new() { Name = "Tours" } }
            });

        // ───────────────────── 7) Tours - Check User Reservation Eligibility ─────────────────────
        group.MapGet("/{tourId:guid}/can-reserve", async (
                [FromRoute] Guid tourId,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetTourDetailQuery
                {
                    TourId = tourId,
                    IncludeUserInfo = true,
                    IncludeStatistics = false,
                    IncludeCapacityDetails = true,
                    IncludePricing = false,
                    IncludeMedia = false,
                    IncludeFeatures = false,
                    IncludeRestrictions = true
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                // Return reservation eligibility information
                var eligibilityInfo = new TourReservationEligibilityResponse
                {
                    TourId = result.Data?.Id ?? Guid.Empty,
                    Title = result.Data?.Title ?? string.Empty,
                    CanReserve = result.Data?.CanUserReserve ?? false,
                    BlockReasons = result.Data?.ReservationBlockReasons ?? new List<string>(),
                    UserReservationStatus = result.Data?.UserReservationStatus,
                    UserReservationId = result.Data?.UserReservationId,
                    UserReservationTrackingCode = result.Data?.UserReservationTrackingCode,
                    IsRegistrationOpen = result.Data?.IsRegistrationOpen ?? false,
                    RegistrationEnd = result.Data?.RegistrationEnd,
                    RemainingCapacity = result.Data?.RemainingCapacity ?? 0,
                    IsFullyBooked = result.Data?.IsFullyBooked ?? false,
                    RestrictedTours = result.Data?.RestrictedTours ?? new List<RestrictedTourDetailDto>()
                };
                
                return Results.Ok(eligibilityInfo);
            })
            .WithName("CheckTourReservationEligibility")
            .Produces<TourReservationEligibilityResponse>(200)
            .Produces(400)
            .Produces(401)
            .Produces(404)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Check Tour Reservation Eligibility",
                Description = "Checks if the current user can reserve a specific tour and returns eligibility information.",
                Tags = new List<OpenApiTag> { new() { Name = "Tours" } }
            });
      
        return app;
    }
    
    
}