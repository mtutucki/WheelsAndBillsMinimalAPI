using WheelsAndBillsAPI.Domain.Entities.Vehicles;

namespace WheelsAndBillsAPI.Domain.Entities.Events
{
    public class VehicleEvent
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid EventTypeId { get; set; }

        public DateTime EventDate { get; set; }
        public int Mileage { get; set; }
        public string? Description { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
        public EventType EventType { get; set; } = null!;
    }

}
