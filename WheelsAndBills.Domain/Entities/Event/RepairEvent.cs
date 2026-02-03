namespace WheelsAndBills.Domain.Entities.Events
{
    public class RepairEvent
    {
        public Guid Id { get; set; }
        public Guid VehicleEventId { get; set; }
        public decimal LaborCost { get; set; }

        public VehicleEvent VehicleEvent { get; set; } = null!;
        public ICollection<EventPart> Parts { get; set; } = new List<EventPart>();
    }

}
