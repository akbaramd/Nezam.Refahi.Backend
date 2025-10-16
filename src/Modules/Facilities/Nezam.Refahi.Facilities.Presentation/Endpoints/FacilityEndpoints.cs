using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.ApproveFacilityRequest;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CancelFacilityRequest;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CreateFacilityRequest;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.RejectFacilityRequest;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCyclesWithRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Presentation.Endpoints;

public static class FacilityEndpoints
{
    public static WebApplication MapFacilityEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/facilities")
            .WithTags("Facilities");

        // ───────────────────── Get Facilities List ─────────────────────
        group.MapGet("", async (
                [FromServices] IMediator mediator,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] string? type = null,
                [FromQuery] string? status = null,
                [FromQuery] string? searchTerm = null,
                [FromQuery] bool onlyActive = true) =>
            {
                var query = new GetFacilitiesQuery
                {
                    Page = page,
                    PageSize = pageSize,
                    Type = type,
                    Status = status,
                    SearchTerm = searchTerm,
                    OnlyActive = onlyActive
                };

                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetFacilities")
            .Produces<ApplicationResult<GetFacilitiesResult>>()
            .Produces(400);

        // ───────────────────── Get Facility Details ─────────────────────
        group.MapGet("/{facilityId:guid}", async (
                Guid facilityId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] bool includeCycles = true,
                [FromQuery] bool includeFeatures = true,
                [FromQuery] bool includePolicies = true,
                [FromQuery] bool includeUserRequestHistory = false) =>
            {
                var query = new GetFacilityDetailsQuery
                {
                    FacilityId = facilityId,
                    IncludeCycles = includeCycles,
                    IncludeFeatures = includeFeatures,
                    IncludePolicies = includePolicies,
                    NationalNumber = currentUserService.UserNationalNumber,
                    IncludeUserRequestHistory = includeUserRequestHistory
                };

                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                if (result.Errors.Any(e => e.Contains("not found")))
                {
                    return Results.NotFound(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetFacilityDetails")
            .Produces<ApplicationResult<GetFacilityDetailsResult>>()
            .Produces(404)
            .Produces(400);

        // ───────────────────── Get Facility Cycles ─────────────────────
        group.MapGet("/{facilityId:guid}/cycles", async (
                Guid facilityId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] string? status = null,
                [FromQuery] bool onlyActive = true,
                [FromQuery] bool onlyEligible = false,
                [FromQuery] bool onlyWithUserRequests = false,
                [FromQuery] bool includeUserRequestStatus = true,
                [FromQuery] bool includeDetailedRequestInfo = false,
                [FromQuery] bool includeStatistics = true) =>
            {
                var query = new GetFacilityCyclesQuery
                {
                    FacilityId = facilityId,
                    NationalNumber = currentUserService.UserNationalNumber,
                    Page = page,
                    PageSize = pageSize,
                    Status = status,
                    OnlyActive = onlyActive,
                    OnlyEligible = onlyEligible,
                    OnlyWithUserRequests = onlyWithUserRequests,
                    IncludeUserRequestStatus = includeUserRequestStatus,
                    IncludeDetailedRequestInfo = includeDetailedRequestInfo,
                    IncludeStatistics = includeStatistics
                };
                
                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                if (result.Errors.Any(e => e.Contains("not found")))
                {
                    return Results.NotFound(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetFacilityCycles")
            .Produces<ApplicationResult<GetFacilityCyclesResponse>>()
            .Produces(404)
            .Produces(400);

        // ───────────────────── Get Facility Cycle Details ─────────────────────
        group.MapGet("/cycles/{cycleId:guid}", async (
                Guid cycleId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] bool includeFacilityInfo = true,
                [FromQuery] bool includeUserRequestHistory = true,
                [FromQuery] bool includeEligibilityDetails = true,
                [FromQuery] bool includeDependencies = true,
                [FromQuery] bool includeStatistics = true) =>
            {
                var query = new GetFacilityCycleDetailsQuery
                {
                    CycleId = cycleId,
                    NationalNumber = currentUserService.UserNationalNumber,
                    IncludeFacilityInfo = includeFacilityInfo,
                    IncludeUserRequestHistory = includeUserRequestHistory,
                    IncludeEligibilityDetails = includeEligibilityDetails,
                    IncludeDependencies = includeDependencies,
                    IncludeStatistics = includeStatistics
                };

                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                if (result.Errors.Any(e => e.Contains("not found")))
                {
                    return Results.NotFound(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetFacilityCycleDetails")
            .Produces<ApplicationResult<GetFacilityCycleDetailsResponse>>()
            .Produces(404)
            .Produces(400);

        // ───────────────────── Get User Cycle Requests ─────────────────────
        group.MapGet("/user/cycle-requests", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] Guid? facilityId = null,
                [FromQuery] Guid? facilityCycleId = null,
                [FromQuery] string? status = null,
                [FromQuery] int? statusCategory = null,
                [FromQuery] DateTime? dateFrom = null,
                [FromQuery] DateTime? dateTo = null,
                [FromQuery] bool includeFacilityInfo = true,
                [FromQuery] bool includeCycleInfo = true,
                [FromQuery] bool includeTimeline = true) =>
            {
                var query = new GetUserCycleRequestsQuery
                {
                    NationalNumber = currentUserService.UserNationalNumber ?? string.Empty,
                    Page = page,
                    PageSize = pageSize,
                    FacilityId = facilityId,
                    FacilityCycleId = facilityCycleId,
                    Status = status,
                    StatusCategory = statusCategory.HasValue ? (Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests.RequestStatusCategory)statusCategory.Value : null,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    IncludeFacilityInfo = includeFacilityInfo,
                    IncludeCycleInfo = includeCycleInfo,
                    IncludeTimeline = includeTimeline
                };

                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetUserCycleRequests")
            .Produces<ApplicationResult<GetUserCycleRequestsResponse>>()
            .Produces(400);

        // ───────────────────── Get User Cycles With Requests ─────────────────────
        group.MapGet("/user/cycles-with-requests", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] Guid? facilityId = null,
                [FromQuery] string? requestStatus = null,
                [FromQuery] int? requestStatusCategory = null,
                [FromQuery] bool onlyActive = true,
                [FromQuery] bool includeDetailedRequestInfo = true,
                [FromQuery] bool includeFacilityInfo = true,
                [FromQuery] bool includeStatistics = true) =>
            {
                var query = new GetUserCyclesWithRequestsQuery
                {
                    NationalNumber = currentUserService.UserNationalNumber ?? string.Empty,
                    FacilityId = facilityId,
                    Page = page,
                    PageSize = pageSize,
                    RequestStatus = requestStatus,
                    RequestStatusCategory = requestStatusCategory.HasValue ? (Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCyclesWithRequests.RequestStatusCategory)requestStatusCategory.Value : null,
                    OnlyActive = onlyActive,
                    IncludeDetailedRequestInfo = includeDetailedRequestInfo,
                    IncludeFacilityInfo = includeFacilityInfo,
                    IncludeStatistics = includeStatistics
                };

                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetUserCyclesWithRequests")
            .Produces<ApplicationResult<GetUserCyclesWithRequestsResponse>>()
            .Produces(400);

        // ───────────────────── Get User Member Info ─────────────────────
        group.MapGet("/user/member-info", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var query = new GetUserMemberInfoQuery
                {
                    NationalNumber = currentUserService.UserNationalNumber ?? string.Empty
                };

                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                if (result.Errors.Any(e => e.Contains("not found")))
                {
                    return Results.NotFound(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetUserMemberInfo")
            .Produces<ApplicationResult<GetUserMemberInfoResponse>>()
            .Produces(404)
            .Produces(400);

        // ───────────────────── Check User Request for Cycle ─────────────────────
        group.MapGet("/cycles/{cycleId:guid}/user-request", async (
                Guid cycleId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var query = new GetFacilityCycleDetailsQuery
                {
                    CycleId = cycleId,
                    NationalNumber = currentUserService.UserNationalNumber,
                    IncludeFacilityInfo = false,
                    IncludeUserRequestHistory = true,
                    IncludeEligibilityDetails = false,
                    IncludeDependencies = false,
                    IncludeStatistics = false
                };

                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                if (result.Errors.Any(e => e.Contains("not found")))
                {
                    return Results.NotFound(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetUserRequestForCycle")
            .Produces<ApplicationResult<GetFacilityCycleDetailsResponse>>()
            .Produces(404)
            .Produces(400);

        // ───────────────────── Get Facility Requests ─────────────────────
        group.MapGet("/requests", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] Guid? facilityId = null,
                [FromQuery] Guid? facilityCycleId = null,
                
                [FromQuery] string? status = null,
                [FromQuery] string? searchTerm = null,
                [FromQuery] DateTime? dateFrom = null,
                [FromQuery] DateTime? dateTo = null) =>
            {
                var query = new GetFacilityRequestsQuery
                {
                    Page = page,
                    PageSize = pageSize,
                    FacilityId = facilityId,
                    FacilityCycleId = facilityCycleId,
                     NationalNumber= currentUserService.UserNationalNumber,
                    Status = status,
                    SearchTerm = searchTerm,
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetFacilityRequests")
            .Produces<ApplicationResult<GetFacilityRequestsResult>>()
            .Produces(400);

        // ───────────────────── Get Facility Request Details ─────────────────────
        group.MapGet("/requests/{requestId:guid}", async (
                Guid requestId,
                [FromServices] IMediator mediator,
                [FromQuery] bool includeFacility = true,
                [FromQuery] bool includeCycle = true,
                [FromQuery] bool includePolicySnapshot = true) =>
            {
                var query = new GetFacilityRequestDetailsQuery
                {
                    RequestId = requestId,
                    IncludeFacility = includeFacility,
                    IncludeCycle = includeCycle,
                    IncludePolicySnapshot = includePolicySnapshot
                };

                var result = await mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                if (result.Errors.Any(e => e.Contains("not found")))
                {
                    return Results.NotFound(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("GetFacilityRequestDetails")
            .Produces<ApplicationResult<GetFacilityRequestDetailsResult>>()
            .Produces(404)
            .Produces(400);

        // ───────────────────── Create Facility Request ─────────────────────
        group.MapPost("/requests", async (
                [FromBody] CreateFacilityRequestCommand command,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                // Set national number from current user service
                var commandWithUser = command with { NationalNumber = currentUserService.UserNationalNumber };
                
                var result = await mediator.Send(commandWithUser);

                if (result.IsSuccess)
                {
                    return Results.Created($"/api/v1/facilities/requests/{result.Data?.RequestId}", result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("CreateFacilityRequest")
            .Produces<ApplicationResult<CreateFacilityRequestResult>>(201)
            .Produces(400);

        // ───────────────────── Approve Facility Request ─────────────────────
        group.MapPost("/requests/{requestId:guid}/approve", async (
                Guid requestId,
                [FromBody] ApproveFacilityRequestRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new ApproveFacilityRequestCommand
                {
                    RequestId = requestId,
                    ApprovedAmountRials = request.ApprovedAmountRials,
                    Currency = request.Currency,
                    Notes = request.Notes,
                    ApproverUserId = request.ApproverUserId
                };

                var result = await mediator.Send(command);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                if (result.Errors.Any(e => e.Contains("not found")))
                {
                    return Results.NotFound(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("ApproveFacilityRequest")
            .Produces<ApplicationResult<ApproveFacilityRequestResult>>()
            .Produces(404)
            .Produces(400);

        // ───────────────────── Reject Facility Request ─────────────────────
        group.MapPost("/requests/{requestId:guid}/reject", async (
                Guid requestId,
                [FromBody] RejectFacilityRequestRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new RejectFacilityRequestCommand
                {
                    RequestId = requestId,
                    Reason = request.Reason,
                    RejectorUserId = request.RejectorUserId
                };

                var result = await mediator.Send(command);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                if (result.Errors.Any(e => e.Contains("not found")))
                {
                    return Results.NotFound(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("RejectFacilityRequest")
            .Produces<ApplicationResult<RejectFacilityRequestResult>>()
            .Produces(404)
            .Produces(400);

        // ───────────────────── Cancel Facility Request ─────────────────────
        group.MapPost("/requests/{requestId:guid}/cancel", async (
                Guid requestId,
                [FromBody] CancelFacilityRequestRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new CancelFacilityRequestCommand
                {
                    RequestId = requestId,
                    Reason = request.Reason,
                    CancelledByUserId = request.CancelledByUserId
                };

                var result = await mediator.Send(command);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                if (result.Errors.Any(e => e.Contains("not found")))
                {
                    return Results.NotFound(result);
                }

                return Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("CancelFacilityRequest")
            .Produces<ApplicationResult<CancelFacilityRequestResult>>()
            .Produces(404)
            .Produces(400);

        return app;
    }
}

// Request DTOs for API endpoints
public record ApproveFacilityRequestRequest
{
    public decimal ApprovedAmountRials { get; init; }
    public string Currency { get; init; } = "IRR";
    public string? Notes { get; init; }
    public Guid ApproverUserId { get; init; }
}

public record RejectFacilityRequestRequest
{
    public string Reason { get; init; } = null!;
    public Guid RejectorUserId { get; init; }
}

public record CancelFacilityRequestRequest
{
    public string? Reason { get; init; }
    public Guid CancelledByUserId { get; init; }
}
