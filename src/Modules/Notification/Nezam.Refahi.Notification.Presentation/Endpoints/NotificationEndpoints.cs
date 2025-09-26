// -----------------------------------------------------------------------------
// Notification.Presentation/Endpoints/NotificationEndpoints.cs
// -----------------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkAsRead;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkAllAsRead;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkByContextAsRead;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkByActionAsRead;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUnreadCount;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUserNotifications;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Presentation.Endpoints;

public static class NotificationEndpoints
{
    public static WebApplication MapNotificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/notifications")
            .WithTags("Notifications")
            .RequireAuthorization();

        // ───────────────────── 1) Notifications - Create ─────────────────────
       
        // ───────────────────── 2) Notifications - Get User Notifications (Paginated) ─────────────────────
        group.MapGet("/user/paginated", async (
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromQuery] bool? isRead,
                [FromQuery] string? context,
                [FromQuery] string? action,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var externalUserId = currentUserService.UserId;
                    if (externalUserId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }

                var query = new GetUserNotificationsQuery
                {
                    ExternalUserId = externalUserId!.Value,
                    PageNumber = pageNumber <= 0 ? 1 : pageNumber,
                    PageSize = pageSize <= 0 ? 20 : pageSize,
                    IsRead = isRead,
                    Context = context,
                    Action = action
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok(result);
            })
            .WithName("GetUserNotificationsPaginated")
            .Produces<ApplicationResult<PaginatedResult<NotificationDto>>>()
            .Produces(400)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get User Notifications (Paginated)",
                Description = "Returns a paginated list of notifications for a specific user with optional filtering.",
                Tags = new List<OpenApiTag> { new() { Name = "Notifications" } }
            });

        // ───────────────────── 3) Notifications - Get All User Notifications ─────────────────────
        group.MapGet("/user", async (
                [FromQuery] bool? isRead,
                [FromQuery] string? context,
                [FromQuery] string? action,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var externalUserId = currentUserService.UserId;
                if (externalUserId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }

                // For now, we'll use the paginated query with a large page size
                // Later, you might want to create a dedicated GetAllUserNotificationsQuery
                var query = new GetUserNotificationsQuery
                {
                    ExternalUserId = externalUserId!.Value,
                    PageNumber = 1,
                    PageSize = 1000, // Large enough to get all notifications
                    IsRead = isRead,
                    Context = context,
                    Action = action
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok(result);
            })
            .WithName("GetAllUserNotifications")
            .Produces<ApplicationResult<PaginatedResult<NotificationDto>>>()
            .Produces(400)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get All User Notifications",
                Description = "Returns all notifications for a specific user with optional filtering.",
                Tags = new List<OpenApiTag> { new() { Name = "Notifications" } }
            });

        // ───────────────────── 4) Notifications - Get Unread Count ─────────────────────
        group.MapGet("/user/unread-count", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var externalUserId = currentUserService.UserId;
                if (externalUserId == null || externalUserId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }

                var query = new GetUnreadCountQuery
                {
                    ExternalUserId = externalUserId!.Value
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok(result);
            })
            .WithName("GetUnreadCount")
            .Produces<ApplicationResult<UnreadCountResponse>>()
            .Produces(400)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Unread Count",
                Description = "Gets the count of unread notifications for a specific user.",
                Tags = new List<OpenApiTag> { new() { Name = "Notifications" } }
            });

        // ───────────────────── 5) Notifications - Get Unread Count by Context ─────────────────────
        group.MapGet("/user/unread-count/context", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var externalUserId = currentUserService.UserId;
                if (externalUserId == null || externalUserId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }

                var query = new GetUnreadCountQuery
                {
                    ExternalUserId = externalUserId!.Value,
                    IncludeContextBreakdown = true
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok(result);
            })
            .WithName("GetUnreadCountByContext")
            .Produces<ApplicationResult<UnreadCountResponse>>()
            .Produces(400)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Unread Count by Context",
                Description = "Gets the count of unread notifications grouped by context for a specific user.",
                Tags = new List<OpenApiTag> { new() { Name = "Notifications" } }
            });

        // ───────────────────── 6) Notifications - Get Unread Count by Action ─────────────────────
        group.MapGet("/user/unread-count/action", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var externalUserId = currentUserService.UserId;
                if (externalUserId == null || externalUserId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }

                var query = new GetUnreadCountQuery
                {
                    ExternalUserId = externalUserId!.Value,
                    IncludeActionBreakdown = true
                };

                var result = await mediator.Send(query);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok(result);
            })
            .WithName("GetUnreadCountByAction")
            .Produces<ApplicationResult<UnreadCountResponse>>()
            .Produces(400)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Unread Count by Action",
                Description = "Gets the count of unread notifications grouped by action for a specific user.",
                Tags = new List<OpenApiTag> { new() { Name = "Notifications" } }
            });

        // ───────────────────── 7) Notifications - Mark as Read ─────────────────────
        group.MapPut("/{notificationId:guid}/read", async (
                [FromRoute] Guid notificationId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var externalUserId = currentUserService.UserId;
                if (externalUserId == null || externalUserId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }

                var command = new MarkAsReadCommand
                {
                    NotificationId = notificationId,
                    ExternalUserId = externalUserId!.Value
                };

                var result = await mediator.Send(command);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok();
            })
            .WithName("MarkAsRead")
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Mark Notification as Read",
                Description = "Marks a specific notification as read for a user.",
                Tags = new List<OpenApiTag> { new() { Name = "Notifications" } }
            });

        // ───────────────────── 8) Notifications - Mark All as Read ─────────────────────
        group.MapPut("/user/mark-all-read", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var externalUserId = currentUserService.UserId;
                if (externalUserId == null || externalUserId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }

                var command = new MarkAllAsReadCommand
                {
                    ExternalUserId = externalUserId!.Value
                };

                var result = await mediator.Send(command);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok();
            })
            .WithName("MarkAllAsRead")
            .Produces(200)
            .Produces(400)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Mark All Notifications as Read",
                Description = "Marks all notifications as read for a specific user.",
                Tags = new List<OpenApiTag> { new() { Name = "Notifications" } }
            });

        // ───────────────────── 9) Notifications - Mark by Context as Read ─────────────────────
        group.MapPut("/user/mark-context-read", async (
                [FromBody] MarkByContextAsReadRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var externalUserId = currentUserService.UserId;
                if (externalUserId == null || externalUserId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }

                var command = new MarkByContextAsReadCommand
                {
                    ExternalUserId = externalUserId!.Value,
                    Context = request.Context
                };

                var result = await mediator.Send(command);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok();
            })
            .WithName("MarkByContextAsRead")
            .Produces(200)
            .Produces(400)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Mark Notifications by Context as Read",
                Description = "Marks all notifications of a specific context as read for a user.",
                Tags = new List<OpenApiTag> { new() { Name = "Notifications" } }
            });

        // ───────────────────── 10) Notifications - Mark by Action as Read ─────────────────────
        group.MapPut("/user/mark-action-read", async (
                [FromBody] MarkByActionAsReadRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var externalUserId = currentUserService.UserId;
                if (externalUserId == null || externalUserId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }

                var command = new MarkByActionAsReadCommand
                {
                    ExternalUserId = externalUserId!.Value,
                    Action = request.Action
                };

                var result = await mediator.Send(command);
                
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                return Results.Ok();
            })
            .WithName("MarkByActionAsRead")
            .Produces(200)
            .Produces(400)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Mark Notifications by Action as Read",
                Description = "Marks all notifications of a specific action as read for a user.",
                Tags = new List<OpenApiTag> { new() { Name = "Notifications" } }
            });

        return app;
    }
}


/// <summary>
/// Request model for mark by context as read
/// </summary>
public class MarkByContextAsReadRequest
{
    public string Context { get; set; } = string.Empty;
}

/// <summary>
/// Request model for mark by action as read
/// </summary>
public class MarkByActionAsReadRequest
{
    public string Action { get; set; } = string.Empty;
}
