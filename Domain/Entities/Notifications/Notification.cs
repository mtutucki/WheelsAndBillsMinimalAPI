using WheelsAndBills.Domain.Entities.Auth;
using WheelsAndBillsAPI.Domain.Entities.Vehicles;

namespace WheelsAndBillsAPI.Domain.Entities.Notification
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid VehicleId { get; set; }

        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime ScheduledAt { get; set; }
        public bool IsSent { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;
    }
}
