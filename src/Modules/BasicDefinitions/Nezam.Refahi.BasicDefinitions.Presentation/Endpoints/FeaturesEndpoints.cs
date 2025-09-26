using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.BasicDefinitions.Application.Services;
using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.BasicDefinitions.Presentation.DTOs;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.BasicDefinitions.Presentation.Endpoints;

/// <summary>
/// Endpoints for Features management
/// </summary>
public static class FeaturesEndpoints
{
    public static void MapFeaturesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/features")
            .WithTags("Features")
            .RequireAuthorization();

        // Get all features
        group.MapGet("/", GetAllFeatures)
            .WithName("GetAllFeatures")
            .WithSummary("Get all features")
            .WithOpenApi();

        // Get active features
        group.MapGet("/active", GetActiveFeatures)
            .WithName("GetActiveFeatures")
            .WithSummary("Get all active features")
            .WithOpenApi();

        // Get features by type
        group.MapGet("/by-type/{type}", GetFeaturesByType)
            .WithName("GetFeaturesByType")
            .WithSummary("Get features by type")
            .WithOpenApi();

        // Get feature by ID
        group.MapGet("/{id}", GetFeatureById)
            .WithName("GetFeatureById")
            .WithSummary("Get feature by ID")
            .WithOpenApi();

        // Get features by keys
        group.MapPost("/by-keys", GetFeaturesByKeys)
            .WithName("GetFeaturesByKeys")
            .WithSummary("Get features by multiple keys")
            .WithOpenApi();

        // Create new feature
        group.MapPost("/", CreateFeature)
            .WithName("CreateFeature")
            .WithSummary("Create a new feature")
            .WithOpenApi();

        // Update feature
        group.MapPut("/{id}", UpdateFeature)
            .WithName("UpdateFeature")
            .WithSummary("Update an existing feature")
            .WithOpenApi();

        // Delete feature
        group.MapDelete("/{id}", DeleteFeature)
            .WithName("DeleteFeature")
            .WithSummary("Delete a feature")
            .WithOpenApi();
    }

    private static async Task<IResult> GetAllFeatures(
        [FromServices] IFeaturesApplicationService service)
    {
        var result = await service.GetAllFeaturesAsync();
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetActiveFeatures(
        [FromServices] IFeaturesApplicationService service)
    {
        var result = await service.GetActiveFeaturesAsync();
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetFeaturesByType(
        string type,
        [FromServices] IFeaturesApplicationService service)
    {
        var result = await service.GetFeaturesByTypeAsync(type);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetFeatureById(
        string id,
        [FromServices] IFeaturesApplicationService service)
    {
        var result = await service.GetFeatureByIdAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> GetFeaturesByKeys(
        [FromBody] List<string> keys,
        [FromServices] IFeaturesApplicationService service)
    {
        var result = await service.GetFeaturesByKeysAsync(keys);
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }

    private static async Task<IResult> CreateFeature(
        [FromBody] CreateFeaturesRequest request,
        [FromServices] IFeaturesApplicationService service)
    {
        // Map request to DTO
        var featureDto = new FeaturesDto
        {
            Id = request.Key,
            Title = request.Title,
            Type = request.Type,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await service.CreateFeatureAsync(featureDto);
        
        return createResult.IsSuccess 
            ? Results.Created($"/api/features/{createResult.Data?.Id}", createResult)
            : Results.BadRequest(createResult);
    }

    private static async Task<IResult> UpdateFeature(
        string id,
        [FromBody] UpdateFeaturesRequest request,
        [FromServices] IFeaturesApplicationService service)
    {
        // Map request to DTO
        var featureDto = new FeaturesDto
        {
            Id = id,
            Title = request.Title,
            Type = request.Type,
            UpdatedAt = DateTime.UtcNow
        };

        var updateResult = await service.UpdateFeatureAsync(featureDto);
        
        return updateResult.IsSuccess 
            ? Results.Ok(updateResult)
            : Results.BadRequest(updateResult);
    }

    private static async Task<IResult> DeleteFeature(
        string id,
        [FromServices] IFeaturesApplicationService service)
    {
        var result = await service.DeleteFeatureAsync(id);
        
        return result.IsSuccess 
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}
