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
/// Endpoints for Capability management
/// </summary>
public static class CapabilityEndpoints
{
    public static void MapCapabilityEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/capabilities")
            .WithTags("Capabilities")
            .RequireAuthorization();

        // Get all capabilities
        group.MapGet("/", GetAllCapabilities)
            .WithName("GetAllCapabilities")
            .WithSummary("Get all capabilities")
            .WithOpenApi();

        // Get active capabilities
        group.MapGet("/active", GetActiveCapabilities)
            .WithName("GetActiveCapabilities")
            .WithSummary("Get all active capabilities")
            .WithOpenApi();

        // Get capabilities by name
        group.MapGet("/by-name/{name}", GetCapabilitiesByName)
            .WithName("GetCapabilitiesByName")
            .WithSummary("Get capabilities by name")
            .WithOpenApi();

        // Get capability by ID
        group.MapGet("/{id}", GetCapabilityById)
            .WithName("GetCapabilityById")
            .WithSummary("Get capability by ID")
            .WithOpenApi();

        // Get capabilities by keys
        group.MapPost("/by-keys", GetCapabilitiesByKeys)
            .WithName("GetCapabilitiesByKeys")
            .WithSummary("Get capabilities by multiple keys")
            .WithOpenApi();

        // Get valid capabilities
        group.MapGet("/valid/{date:datetime}", GetValidCapabilities)
            .WithName("GetValidCapabilities")
            .WithSummary("Get capabilities valid at specific date")
            .WithOpenApi();

        // Create new capability
        group.MapPost("/", CreateCapability)
            .WithName("CreateCapability")
            .WithSummary("Create a new capability")
            .WithOpenApi();

        // Update capability
        group.MapPut("/{id}", UpdateCapability)
            .WithName("UpdateCapability")
            .WithSummary("Update an existing capability")
            .WithOpenApi();

        // Delete capability
        group.MapDelete("/{id}", DeleteCapability)
            .WithName("DeleteCapability")
            .WithSummary("Delete a capability")
            .WithOpenApi();

        // Add features to capability
        group.MapPost("/{id}/features", AddFeaturesToCapability)
            .WithName("AddFeaturesToCapability")
            .WithSummary("Add features to a capability")
            .WithOpenApi();

        // Remove features from capability
        group.MapDelete("/{id}/features", RemoveFeaturesFromCapability)
            .WithName("RemoveFeaturesFromCapability")
            .WithSummary("Remove features from a capability")
            .WithOpenApi();
    }

    private static async Task<IResult> GetAllCapabilities(
        [FromServices] ICapabilityApplicationService service)
    {
        var result = await service.GetAllCapabilitiesAsync();
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetActiveCapabilities(
        [FromServices] ICapabilityApplicationService service)
    {
        var result = await service.GetActiveCapabilitiesAsync();
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetCapabilitiesByName(
        string name,
        [FromServices] ICapabilityApplicationService service)
    {
        var result = await service.GetCapabilitiesByNameAsync(name);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetCapabilityById(
        string id,
        [FromServices] ICapabilityApplicationService service)
    {
        var result = await service.GetCapabilityByIdAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetCapabilitiesByKeys(
        [FromBody] List<string> keys,
        [FromServices] ICapabilityApplicationService service)
    {
        var result = await service.GetCapabilitiesByKeysAsync(keys);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetValidCapabilities(
        DateTime date,
        [FromServices] ICapabilityApplicationService service)
    {
        var result = await service.GetValidCapabilitiesAsync(date);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> CreateCapability(
        [FromBody] CreateCapabilityRequest request,
        [FromServices] ICapabilityApplicationService service)
    {
        // Map request to DTO
        var capabilityDto = new CapabilityDto
        {
            Id = request.Key,
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            Features = request.FeatureIds.Select(id => new FeaturesDto { Id = id }).ToList(),
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await service.CreateCapabilityAsync(capabilityDto);
        
        return createResult.IsSuccess 
            ? Results.Created($"/api/capabilities/{createResult.Data?.Id}", createResult)
            : Results.BadRequest(createResult);
    }

    private static async Task<IResult> UpdateCapability(
        string id,
        [FromBody] UpdateCapabilityRequest request,
        [FromServices] ICapabilityApplicationService service)
    {
        // Map request to DTO
        var capabilityDto = new CapabilityDto
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            Features = request.FeatureIds.Select(featureId => new FeaturesDto { Id = featureId }).ToList(),
            UpdatedAt = DateTime.UtcNow
        };

        var updateResult = await service.UpdateCapabilityAsync(capabilityDto);
        
        return updateResult.IsSuccess 
            ? Results.Ok(updateResult)
            : Results.BadRequest(updateResult);
    }

    private static async Task<IResult> DeleteCapability(
        string id,
        [FromServices] ICapabilityApplicationService service)
    {
        var result = await service.DeleteCapabilityAsync(id);
        
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> AddFeaturesToCapability(
        string id,
        [FromBody] List<string> featureIds,
        [FromServices] ICapabilityApplicationService service)
    {
        var result = await service.AddFeaturesToCapabilityAsync(id, featureIds);
        
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> RemoveFeaturesFromCapability(
        string id,
        [FromBody] List<string> featureIds,
        [FromServices] ICapabilityApplicationService service)
    {
        var result = await service.RemoveFeaturesFromCapabilityAsync(id, featureIds);
        
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}
