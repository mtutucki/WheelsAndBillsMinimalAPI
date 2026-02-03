namespace WheelsAndBills.Domain.Entities.Admin
{
    public class ContentBlock
    {
        public Guid Id { get; set; }
        public Guid ContentPageId { get; set; }
        public string Content { get; set; } = null!;
        public string Slot { get; set; } = null!;
    }
}
