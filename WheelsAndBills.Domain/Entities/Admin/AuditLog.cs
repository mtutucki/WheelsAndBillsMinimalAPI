namespace WheelsAndBills.Domain.Entities.Admin
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
