using static WheelsAndBills.Application.DTOs.Notifications.NotificationsDTOs;
using WheelsAndBills.Application.Features.Notifications.NotificationTypes;

namespace WheelsAndBills.API.Endpoints.Notifications.NotificationTypes
{
    public static class NotificationTypesEndpoints
    {
        public static RouteHandlerBuilder MapGetNotificationTypes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                INotificationTypesService notificationTypes,
                CancellationToken cancellationToken) =>
            {
                var types = await notificationTypes.GetAllAsync(cancellationToken);

                return Results.Ok(types);
            });
        }

        public static RouteHandlerBuilder MapGetNotificationTypeById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                INotificationTypesService notificationTypes,
                CancellationToken cancellationToken) =>
            {
                var type = await notificationTypes.GetByIdAsync(id, cancellationToken);

                return type is null
                    ? Results.NotFound()
                    : Results.Ok(type);
            });
        }

        public static RouteHandlerBuilder MapCreateNotificationType(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateNotificationTypeDTO request,
                INotificationTypesService notificationTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await notificationTypes.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/notification-types/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateNotificationType(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateNotificationTypeDTO request,
                INotificationTypesService notificationTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await notificationTypes.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteNotificationType(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                INotificationTypesService notificationTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await notificationTypes.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
