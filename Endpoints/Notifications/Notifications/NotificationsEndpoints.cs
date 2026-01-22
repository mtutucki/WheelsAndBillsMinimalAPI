using static WheelsAndBillsAPI.Endpoints.Notifications.NotificationsDTOs;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Notifications.Notifications
{
    public static class NotificationsEndpoints
    {
        public static RouteHandlerBuilder MapGetNotifications(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var notifications = await db.Notifications
                    .Select(n => new GetNotificationDTO(
                        n.Id,
                        n.UserId,
                        n.VehicleId,
                        n.Title,
                        n.Message,
                        n.ScheduledAt,
                        n.IsSent
                    ))
                    .ToListAsync();

                return Results.Ok(notifications);
            });
        }

        public static RouteHandlerBuilder MapGetNotificationById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var notification = await db.Notifications
                    .Where(n => n.Id == id)
                    .Select(n => new GetNotificationDTO(
                        n.Id,
                        n.UserId,
                        n.VehicleId,
                        n.Title,
                        n.Message,
                        n.ScheduledAt,
                        n.IsSent
                    ))
                    .FirstOrDefaultAsync();

                return notification is null
                    ? Results.NotFound()
                    : Results.Ok(notification);
            });
        }

        public static RouteHandlerBuilder MapCreateNotification(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateNotificationDTO request,
                AppDbContext db) =>
            {
                var userExists = await db.Users.AnyAsync(u => u.Id == request.UserId);
                if (!userExists)
                    return Results.BadRequest("User does not exist");

                var vehicleExists = await db.Vehicles.AnyAsync(v => v.Id == request.VehicleId);
                if (!vehicleExists)
                    return Results.BadRequest("Vehicle does not exist");

                var notification = new Domain.Entities.Notification.Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    VehicleId = request.VehicleId,
                    Title = request.Title,
                    Message = request.Message,
                    ScheduledAt = request.ScheduledAt,
                    IsSent = false
                };

                db.Notifications.Add(notification);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/notifications/{notification.Id}",
                    new GetNotificationDTO(
                        notification.Id,
                        notification.UserId,
                        notification.VehicleId,
                        notification.Title,
                        notification.Message,
                        notification.ScheduledAt,
                        notification.IsSent
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateNotification(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateNotificationDTO request,
                AppDbContext db) =>
            {
                var notification = await db.Notifications.FindAsync(id);
                if (notification is null)
                    return Results.NotFound();

                notification.Title = request.Title;
                notification.Message = request.Message;
                notification.ScheduledAt = request.ScheduledAt;
                notification.IsSent = request.IsSent;

                await db.SaveChangesAsync();

                return Results.Ok(new GetNotificationDTO(
                    notification.Id,
                    notification.UserId,
                    notification.VehicleId,
                    notification.Title,
                    notification.Message,
                    notification.ScheduledAt,
                    notification.IsSent
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteNotification(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var notification = await db.Notifications.FindAsync(id);
                if (notification is null)
                    return Results.NotFound();

                db.Notifications.Remove(notification);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}

