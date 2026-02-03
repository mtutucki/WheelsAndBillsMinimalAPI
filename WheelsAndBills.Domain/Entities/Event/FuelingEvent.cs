namespace WheelsAndBills.Domain.Entities.Events
{
    public class FuelingEvent
    {
        public Guid Id { get; set; }
        public Guid VehicleEventId { get; set; }
        public decimal Liters { get; set; }
        public decimal TotalPrice { get; set; }

        public VehicleEvent VehicleEvent { get; set; } = null!;
    }

}
