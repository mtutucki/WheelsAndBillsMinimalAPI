namespace WheelsAndBills.Domain.Entities.Admin
{
    public class ErrorLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Source { get; set; } = "Server";
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? Path { get; set; }
        public string? Method { get; set; }
        public int? StatusCode { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
