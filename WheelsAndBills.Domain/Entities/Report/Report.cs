using WheelsAndBills.Domain.Entities.Auth;

namespace WheelsAndBills.Domain.Entities.Report
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ReportDefinitionId { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public ReportDefinition Definition { get; set; } = null!;
    }

}
