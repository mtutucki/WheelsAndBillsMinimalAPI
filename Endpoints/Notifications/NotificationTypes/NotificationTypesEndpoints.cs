using static WheelsAndBillsAPI.Endpoints.Notifications.NotificationsDTOs;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Notifications.NotificationTypes
{
    public static class NotificationTypesEndpoints
    {
        public static RouteHandlerBuilder MapGetNotificationTypes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var types = await db.NotificationTypes
                    .OrderBy(t => t.Code)
                    .Select(t => new GetNotificationTypeDTO(t.Id, t.Code))
                    .ToListAsync();

                return Results.Ok(types);
            });
        }

        public static RouteHandlerBuilder MapGetNotificationTypeById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var type = await db.NotificationTypes
                    .Where(t => t.Id == id)
                    .Select(t => new GetNotificationTypeDTO(t.Id, t.Code))
                    .FirstOrDefaultAsync();

                return type is null
                    ? Results.NotFound()
                    : Results.Ok(type);
            });
        }

        public static RouteHandlerBuilder MapCreateNotificationType(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateNotificationTypeDTO request,
                AppDbContext db) =>
            {
                var exists = await db.NotificationTypes
                    .AnyAsync(t => t.Code == request.Code);

                if (exists)
                    return Results.BadRequest("NotificationType already exists");

                var type = new Domain.Entities.Notification.NotificationType
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code
                };

                db.NotificationTypes.Add(type);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/notification-types/{type.Id}",
                    new GetNotificationTypeDTO(type.Id, type.Code)
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateNotificationType(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateNotificationTypeDTO request,
                AppDbContext db) =>
            {
                var type = await db.NotificationTypes.FindAsync(id);
                if (type is null)
                    return Results.NotFound();

                var exists = await db.NotificationTypes
                    .AnyAsync(t => t.Code == request.Code && t.Id != id);

                if (exists)
                    return Results.BadRequest("NotificationType already exists");

                type.Code = request.Code;
                await db.SaveChangesAsync();

                return Results.Ok(new GetNotificationTypeDTO(type.Id, type.Code));
            });
        }

        public static RouteHandlerBuilder MapDeleteNotificationType(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var type = await db.NotificationTypes.FindAsync(id);
                if (type is null)
                    return Results.NotFound();

                db.NotificationTypes.Remove(type);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
