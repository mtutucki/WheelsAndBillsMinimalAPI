namespace WheelsAndBillsAPI.Domain.Entities.Events
{
    public class EventType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
