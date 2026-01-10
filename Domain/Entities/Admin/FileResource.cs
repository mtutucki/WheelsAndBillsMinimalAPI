namespace WheelsAndBillsAPI.Domain.Entities.Admin
{
    public class FileResource
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}


