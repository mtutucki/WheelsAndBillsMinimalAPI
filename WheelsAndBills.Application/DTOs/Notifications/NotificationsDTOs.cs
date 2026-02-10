namespace WheelsAndBills.Application.DTOs.Notifications
{
    public class NotificationsDTOs
    {
        public record CreateNotificationDTO(
            Guid UserId,
            Guid VehicleId,
            Guid NotificationTypeId,
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
            Guid? NotificationTypeId,
            string? NotificationTypeCode,
            string Title,
            string Message,
            DateTime ScheduledAt,
            bool IsSent,
            bool IsRead
        );

        public record NotificationPreferenceDTO(
            Guid NotificationTypeId,
            string NotificationTypeCode,
            bool IsEnabled
        );

        public record UpdateNotificationPreferencesDTO(
            IReadOnlyList<NotificationPreferenceDTO> Preferences
        );

        public record CreateNotificationTypeDTO(string Code);

        public record UpdateNotificationTypeDTO(string Code);

        public record GetNotificationTypeDTO(Guid Id, string Code);
    }
}
