using static WheelsAndBills.Application.DTOs.Notifications.NotificationsDTOs;
using WheelsAndBills.Application.Features.Notifications.Notifications;
using WheelsAndBills.Application.Features.Notifications.Preferences;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WheelsAndBills.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBills.API.Endpoints.Notifications.Notifications
{
    public static class NotificationsEndpoints
    {
        public static RouteHandlerBuilder MapGetNotifications(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                INotificationsService notifications,
                CancellationToken cancellationToken) =>
            {
                var notificationsList = await notifications.GetAllAsync(cancellationToken);

                return Results.Ok(notificationsList);
            });
        }

        public static RouteHandlerBuilder MapGetMyNotifications(this RouteGroupBuilder app)
        {
            return app.MapGet("/my", [Authorize] async (
                ClaimsPrincipal user,
                INotificationsService notifications,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim);
                var notificationsList = await notifications.GetForUserAsync(userId, cancellationToken);

                return Results.Ok(notificationsList);
            });
        }

        public static RouteHandlerBuilder MapGetNotificationById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                INotificationsService notifications,
                CancellationToken cancellationToken) =>
            {
                var notification = await notifications.GetByIdAsync(id, cancellationToken);

                return notification is null
                    ? Results.NotFound()
                    : Results.Ok(notification);
            });
        }

        public static RouteHandlerBuilder MapCreateNotification(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateNotificationDTO request,
                INotificationsService notifications,
                CancellationToken cancellationToken) =>
            {
                var result = await notifications.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/notifications/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapGetPreferences(this RouteGroupBuilder app)
        {
            return app.MapGet("/preferences", [Authorize] async (
                ClaimsPrincipal user,
                INotificationPreferencesService preferences,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim);
                var result = await preferences.GetForUserAsync(userId, cancellationToken);

                return Results.Ok(result);
            });
        }

        public static RouteHandlerBuilder MapGetUnreadCount(this RouteGroupBuilder app)
        {
            return app.MapGet("/unread-count", [Authorize] async (
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim);
                var count = await db.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .CountAsync(cancellationToken);

                return Results.Ok(new { Count = count });
            });
        }

        public static RouteHandlerBuilder MapUpdatePreferences(this RouteGroupBuilder app)
        {
            return app.MapPut("/preferences", [Authorize] async (
                ClaimsPrincipal user,
                UpdateNotificationPreferencesDTO request,
                INotificationPreferencesService preferences,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim);
                await preferences.UpdateForUserAsync(userId, request, cancellationToken);

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapUpdateNotification(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateNotificationDTO request,
                INotificationsService notifications,
                CancellationToken cancellationToken) =>
            {
                var result = await notifications.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapMarkAsRead(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}/read", [Authorize] async (
                Guid id,
                ClaimsPrincipal user,
                INotificationsService notifications,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim);
                var result = await notifications.MarkAsReadAsync(id, userId, cancellationToken);

                return result.Success ? Results.NoContent() : Results.NotFound();
            });
        }

        public static RouteHandlerBuilder MapMarkAllAsRead(this RouteGroupBuilder app)
        {
            return app.MapPut("/read-all", [Authorize] async (
                ClaimsPrincipal user,
                INotificationsService notifications,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim);
                await notifications.MarkAllAsReadAsync(userId, cancellationToken);

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapDeleteNotification(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                INotificationsService notifications,
                CancellationToken cancellationToken) =>
            {
                var result = await notifications.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}

