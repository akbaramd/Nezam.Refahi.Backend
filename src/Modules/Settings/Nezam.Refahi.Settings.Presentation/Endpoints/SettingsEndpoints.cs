using MediatR;
using Nezam.Refahi.Settings.Application.Commands.BulkUpdateSettings;
using Nezam.Refahi.Settings.Application.Commands.CreateCategory;
using Nezam.Refahi.Settings.Application.Commands.CreateSection;
using Nezam.Refahi.Settings.Application.Commands.SetSetting;
using Nezam.Refahi.Settings.Application.Commands.UpdateSetting;
using Nezam.Refahi.Settings.Application.Queries.GetSettingByKey;
using Nezam.Refahi.Settings.Application.Queries.GetSettings;
using Nezam.Refahi.Settings.Application.Queries.GetSettingsBySection;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Settings.Presentation.Endpoints;

/// <summary>
/// Minimal API endpoints for Settings management
/// </summary>
public static class SettingsEndpoints
{
    /// <summary>
    /// Maps all settings endpoints
    /// </summary>
    public static void MapSettingsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/settings")
            .WithTags("Settings")
            .WithOpenApi();

        // Section management - Use plural nouns for collections
        group.MapPost("/sections", CreateSection)
            .WithName("CreateSection")
            .WithSummary("Create a new settings section")
            .Produces<CreateSectionResponse>(201)
            .ProducesProblem(400)
            .ProducesProblem(500);

        // Category management - Use plural nouns for collections
        group.MapPost("/categories", CreateCategory)
            .WithName("CreateCategory")
            .WithSummary("Create a new settings category")
            .Produces<CreateCategoryResponse>(201)
            .ProducesProblem(400)
            .ProducesProblem(500);

        // Setting management - Use plural nouns for collections
        group.MapPost("/", SetSetting)
            .WithName("SetSetting")
            .WithSummary("Create or update a setting")
            .Produces<SetSettingResponse>(201)
            .ProducesProblem(400)
            .ProducesProblem(500);

        // Bulk operations - Use query parameters or treat as resource
        group.MapPut("/bulk", BulkUpdateSettings)
            .WithName("BulkUpdateSettings")
            .WithSummary("Bulk update multiple settings")
            .Produces<BulkUpdateSettingsResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(500);

        // Individual resource updates - Use resource ID pattern
        group.MapPut("/{settingId}", UpdateSetting)
            .WithName("UpdateSetting")
            .WithSummary("Update an existing setting value")
            .Produces<UpdateSettingResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500);

        // Collection endpoints - Use plural nouns and query parameters for filtering
        group.MapGet("/", GetSettings)
            .WithName("GetSettings")
            .WithSummary("Get settings with filters and pagination")
            .Produces<GetSettingsResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(500);

                // Organized view - Use descriptive resource names
        group.MapGet("/organized", GetSettingsBySection)   
            .WithName("GetSettingsBySection")
            .WithSummary("Get settings organized by section and category")
            .Produces<GetSettingsBySectionResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(500);

        // Individual resource by key - Use resource identifier pattern
        group.MapGet("/{key:regex(^[a-z0-9-]+$)}", GetSettingByKey)
            .WithName("GetSettingByKey")
            .WithSummary("Get a specific setting by its key")
            .Produces<GetSettingByKeyResponse>(200)
            .ProducesProblem(404)
            .ProducesProblem(500);     
    }

    #region Command Handlers

    /// <summary>
    /// Creates a new settings section
    /// </summary>
    private static async Task<IResult> CreateSection(
        CreateSectionCommand command,
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        try
        {
            var result = await mediator.Send(command);
            return result.IsSuccess 
                ? Results.Created($"/api/v1/settings/sections/{result.Data!.Id}", result.Data)
                : Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new settings category
    /// </summary>
    private static async Task<IResult> CreateCategory(
        CreateCategoryCommand command,
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        try
        {
            var result = await mediator.Send(command);
            return result.IsSuccess 
                ? Results.Created($"/api/v1/settings/categories/{result.Data!.Id}", result.Data)
                : Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    /// <summary>
    /// Creates or updates a setting
    /// </summary>
    private static async Task<IResult> SetSetting(
        SetSettingCommand command,
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        try
        {
            // Set the user ID from current user context
            command = command with { UserId = currentUserService.UserId ?? Guid.Empty };

            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return result.Data!.WasCreated 
                    ? Results.Created($"/api/v1/settings/{result.Data.Id}", result.Data)
                    : Results.Ok(result.Data);
            }
            return Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing setting value
    /// </summary>
    private static async Task<IResult> UpdateSetting(
        Guid settingId,
        UpdateSettingCommand command,
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        try
        {
            // Ensure the setting ID in the command matches the route parameter
            command = command with { SettingId = settingId, UserId = currentUserService.UserId ?? Guid.Empty };

            var result = await mediator.Send(command);
            return result.IsSuccess 
                ? Results.Ok(result.Data!)
                : Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    /// <summary>
    /// Bulk updates multiple settings
    /// </summary>
    private static async Task<IResult> BulkUpdateSettings(
        BulkUpdateSettingsCommand command,
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        try
        {
            // Set the user ID from current user context
            command = command with { UserId = currentUserService.UserId ?? Guid.Empty   };

            var result = await mediator.Send(command);
            return result.IsSuccess 
                ? Results.Ok(result.Data!)
                : Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    #endregion

    #region Query Handlers

    /// <summary>
    /// Gets settings with filters and pagination
    /// </summary>
    private static async Task<IResult> GetSettings(
        [AsParameters] GetSettingsQuery query,
        IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(query);
            return result.IsSuccess 
                ? Results.Ok(result.Data!)
                : Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    /// <summary>
    /// Gets a specific setting by its key
    /// </summary>
    private static async Task<IResult> GetSettingByKey(
        string key,
        [AsParameters] GetSettingByKeyQuery query,
        IMediator mediator)
    {
        try
        {
            // Create a new query with the key from route parameter
            var queryWithKey = query with { SettingKey = key };

            var result = await mediator.Send(queryWithKey);
            if (result.IsSuccess)
            {
                return result.Data!.Found 
                    ? Results.Ok(result.Data)
                    : Results.NotFound();
            }
            return Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    /// <summary>
    /// Gets settings organized by section and category
    /// </summary>
    private static async Task<IResult> GetSettingsBySection(
        [AsParameters] GetSettingsBySectionQuery query,
        IMediator mediator)
    {
        try
        {
            var result = await mediator.Send(query);
            return result.IsSuccess 
                ? Results.Ok(result.Data!)
                : Results.BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    #endregion
}
