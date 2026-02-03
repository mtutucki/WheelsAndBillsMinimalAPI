namespace WheelsAndBills.Domain.Entities.Events
{
    public class ServiceEvent
    {
        public Guid Id { get; set; }
        public Guid VehicleEventId { get; set; }
        public Guid WorkshopId { get; set; }

        public VehicleEvent VehicleEvent { get; set; } = null!;
        public Workshop Workshop { get; set; } = null!;
    }

}
