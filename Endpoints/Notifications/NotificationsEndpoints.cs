using WheelsAndBillsAPI.Endpoints.Notifications.Notifications;
using WheelsAndBillsAPI.Endpoints.Notifications.NotificationTypes;

namespace WheelsAndBillsAPI.Endpoints.Notifications
{
    public static class NotificationsEndpoints
    {
        public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
        {
            var notifications = app
                .MapGroup("/notifications")
                .WithTags("Notifications")
                .RequireAuthorization();

            var notificationsTypes = app
                .MapGroup("/notifications-types")
                .WithTags("Notifications types")
                .RequireAuthorization();

            notifications.MapCreateNotification();
            notifications.MapUpdateNotification();
            notifications.MapDeleteNotification();
            notifications.MapGetNotifications();
            notifications.MapGetNotificationById();

            notificationsTypes.MapCreateNotificationType();
            notificationsTypes.MapUpdateNotificationType();
            notificationsTypes.MapDeleteNotificationType();
            notificationsTypes.MapGetNotificationTypes();
            notificationsTypes.MapGetNotificationTypeById();

            return app;
        }
    }
}
