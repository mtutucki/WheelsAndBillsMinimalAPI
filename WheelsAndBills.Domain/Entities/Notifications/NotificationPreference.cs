using WheelsAndBills.Domain.Entities.Auth;

namespace WheelsAndBills.Domain.Entities.Notification
{
    public class NotificationPreference
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid NotificationTypeId { get; set; }
        public bool IsEnabled { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public NotificationType NotificationType { get; set; } = null!;
    }
}
