namespace WheelsAndBills.Domain.Entities.Events
{
    public class EventPart
    {
        public Guid RepairEventId { get; set; }
        public Guid PartId { get; set; }
        public decimal Price { get; set; }

        public RepairEvent RepairEvent { get; set; } = null!;
        public Part Part { get; set; } = null!;
    }
}
