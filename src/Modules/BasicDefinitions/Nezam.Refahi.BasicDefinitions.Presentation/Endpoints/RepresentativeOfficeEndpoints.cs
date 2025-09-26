using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.BasicDefinitions.Presentation.Endpoints;

/// <summary>
/// Endpoints for RepresentativeOffice management
/// </summary>
public static class RepresentativeOfficeEndpoints
{
    public static void MapRepresentativeOfficeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/representative-offices")
            .WithTags("Representative Offices")
            .RequireAuthorization();

        // Get all active offices
        group.MapGet("/", GetActiveOffices)
            .WithName("GetActiveOffices")
            .WithSummary("Get all active representative offices")
            .WithOpenApi();

        // Get office by ID
        group.MapGet("/{id:guid}", GetOfficeById)
            .WithName("GetOfficeById")
            .WithSummary("Get representative office by ID")
            .WithOpenApi();

        // Get office by code
        group.MapGet("/by-code/{code}", GetOfficeByCode)
            .WithName("GetOfficeByCode")
            .WithSummary("Get representative office by code")
            .WithOpenApi();

        // Get office by external code
        group.MapGet("/by-external-code/{externalCode}", GetOfficeByExternalCode)
            .WithName("GetOfficeByExternalCode")
            .WithSummary("Get representative office by external code")
            .WithOpenApi();
    }

    private static async Task<IResult> GetActiveOffices(
        [FromServices] IRepresentativeOfficeService service)
    {
        var offices = await service.GetActiveOfficesAsync();
        return Results.Ok(offices);
    }

    private static async Task<IResult> GetOfficeById(
        Guid id,
        [FromServices] IRepresentativeOfficeService service)
    {
        var office = await service.GetOfficeByIdAsync(id);
        return office != null ? Results.Ok(office) : Results.NotFound();
    }

    private static async Task<IResult> GetOfficeByCode(
        string code,
        [FromServices] IRepresentativeOfficeService service)
    {
        var office = await service.GetOfficeByCodeAsync(code);
        return office != null ? Results.Ok(office) : Results.NotFound();
    }

    private static async Task<IResult> GetOfficeByExternalCode(
        string externalCode,
        [FromServices] IRepresentativeOfficeService service)
    {
        var office = await service.GetOfficeByExternalCodeAsync(externalCode);
        return office != null ? Results.Ok(office) : Results.NotFound();
    }
}