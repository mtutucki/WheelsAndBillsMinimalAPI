using WheelsAndBills.Domain.Entities.Auth;

namespace WheelsAndBills.Domain.Entities.Vehicles
{
    public class VehicleNote
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid UserId { get; set; }

        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
