using static WheelsAndBills.Application.DTOs.Notifications.NotificationsDTOs;
using WheelsAndBills.Application.Features.Notifications.Notifications;

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

