using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.BasicDefinitions.Application.Services;
using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.BasicDefinitions.Presentation.DTOs;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.BasicDefinitions.Presentation.Endpoints;

/// <summary>
/// Endpoints for Agency management
/// </summary>
public static class AgencyEndpoints
{
    public static void MapAgencyEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/representative-offices")
            .WithTags("Representative Offices")
            .RequireAuthorization();

        // Get all active offices
        group.MapGet("/", GetActiveOffices)
            .WithName("GetActiveOffices")
            .WithSummary("Get all active representative offices")
            .WithOpenApi();

        // Get all offices (including inactive)
        group.MapGet("/all", GetAllOffices)
            .WithName("GetAllOffices")
            .WithSummary("Get all representative offices (including inactive)")
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

        // Create new office
        group.MapPost("/", CreateOffice)
            .WithName("CreateOffice")
            .WithSummary("Create a new representative office")
            .WithOpenApi();

        // Update office
        group.MapPut("/{id:guid}", UpdateOffice)
            .WithName("UpdateOffice")
            .WithSummary("Update an existing representative office")
            .WithOpenApi();

        // Delete office
        group.MapDelete("/{id:guid}", DeleteOffice)
            .WithName("DeleteOffice")
            .WithSummary("Delete a representative office")
            .WithOpenApi();
    }

    private static async Task<IResult> GetActiveOffices(
        [FromServices] IAgencyApplicationService service)
    {
        var result = await service.GetActiveOfficesAsync();
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetAllOffices(
        [FromServices] IAgencyApplicationService service)
    {
        var result = await service.GetAllOfficesAsync();
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetOfficeById(
        Guid id,
        [FromServices] IAgencyApplicationService service)
    {
        var result = await service.GetOfficeByIdAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetOfficeByCode(
        string code,
        [FromServices] IAgencyApplicationService service)
    {
        var result = await service.GetOfficeByCodeAsync(code);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetOfficeByExternalCode(
        string externalCode,
        [FromServices] IAgencyApplicationService service)
    {
        var result = await service.GetOfficeByExternalCodeAsync(externalCode);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> CreateOffice(
        [FromBody] CreateAgencyRequest request,
        [FromServices] IAgencyApplicationService service)
    {
        // Map request to DTO
        var officeDto = new AgencyDto
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            ExternalCode = request.ExternalCode,
            Name = request.Name,
            Address = request.Address,
            ManagerName = request.ManagerName,
            ManagerPhone = request.ManagerPhone,
            IsActive = true,
            EstablishedDate = request.EstablishedDate,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await service.CreateOfficeAsync(officeDto);
        
        return createResult.IsSuccess 
            ? Results.Created($"/api/representative-offices/{createResult.Data?.Id}", createResult)
            : Results.BadRequest(createResult);
    }

    private static async Task<IResult> UpdateOffice(
        Guid id,
        [FromBody] UpdateAgencyRequest request,
        [FromServices] IAgencyApplicationService service)
    {
        // Map request to DTO
        var officeDto = new AgencyDto
        {
            Id = id,
            Code = request.Code,
            ExternalCode = request.ExternalCode,
            Name = request.Name,
            Address = request.Address,
            ManagerName = request.ManagerName,
            ManagerPhone = request.ManagerPhone,
            IsActive = request.IsActive,
            EstablishedDate = request.EstablishedDate,
            UpdatedAt = DateTime.UtcNow
        };

        var updateResult = await service.UpdateOfficeAsync(officeDto);
        
        return updateResult.IsSuccess 
            ? Results.Ok(updateResult)
            : Results.BadRequest(updateResult);
    }

    private static async Task<IResult> DeleteOffice(
        Guid id,
        [FromServices] IAgencyApplicationService service)
    {
        var result = await service.DeleteOfficeAsync(id);
        
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}
