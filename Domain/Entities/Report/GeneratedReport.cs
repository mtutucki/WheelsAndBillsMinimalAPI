namespace WheelsAndBillsAPI.Domain.Entities.Report
{
    public class GeneratedReport
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public string FilePath { get; set; } = null!;

        public Report Report { get; set; } = null!;
    }
}
