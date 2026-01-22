namespace WheelsAndBillsAPI.Endpoints.Notifications
{
    public class NotificationsDTOs
    {
        public record CreateNotificationDTO(
            Guid UserId,
            Guid VehicleId,
            string Title,
            string Message,
            DateTime ScheduledAt
        );

        public record UpdateNotificationDTO(
            string Title,
            string Message,
            DateTime ScheduledAt,
            bool IsSent
        );

        public record GetNotificationDTO(
            Guid Id,
            Guid UserId,
            Guid VehicleId,
            string Title,
            string Message,
            DateTime ScheduledAt,
            bool IsSent
        );

        public record CreateNotificationTypeDTO(string Code);

        public record UpdateNotificationTypeDTO(string Code);

        public record GetNotificationTypeDTO(Guid Id, string Code);
    }
}
