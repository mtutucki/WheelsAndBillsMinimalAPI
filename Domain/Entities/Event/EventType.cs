namespace WheelsAndBillsAPI.Domain.Entities.Events
{
    public class EventType
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
    }
}
