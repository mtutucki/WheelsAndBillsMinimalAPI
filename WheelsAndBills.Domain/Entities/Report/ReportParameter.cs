namespace WheelsAndBills.Domain.Entities.Report
{
    public class ReportParameter
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;

        public Report Report { get; set; } = null!;
    }

}


