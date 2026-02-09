namespace WheelsAndBills.Application.DTOs.Reports
{
    public class ReportDTOs
    {
        public record CreateGeneratedReportDTO(
            Guid ReportId,
            string FilePath
        );

        public record UpdateGeneratedReportDTO(
            string FilePath
        );

        public record GetGeneratedReportDTO(
            Guid Id,
            Guid ReportId,
            string FilePath
        );

        public record CreateReportDTO(
        Guid UserId,
        Guid ReportDefinitionId
    );

        public record UpdateReportDTO(
            Guid ReportDefinitionId
        );

        public record GetReportDTO(
            Guid Id,
            Guid UserId,
            Guid ReportDefinitionId,
            DateTime CreatedAt
        );


        public record CreateReportDefinitionDTO(string Code);

        public record UpdateReportDefinitionDTO(string Code);

        public record GetReportDefinitionDTO(Guid Id, string Code);


        public record CreateReportParameterDTO(
            Guid ReportId,
            string Name,
            string Value
        );

        public record UpdateReportParameterDTO(
            string Name,
            string Value
        );

        public record GetReportParameterDTO(
            Guid Id,
            Guid ReportId,
            string Name,
            string Value
        );

        public class MonthlyCostRow
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal TotalAmount { get; set; }
        }

        public class CostsByEventTypeRow
        {
            public string EventType { get; set; } = null!;
            public decimal TotalAmount { get; set; }
        }

        public class RepairHistoryRow
        {
            public DateTime EventDate { get; set; }
            public Guid RepairEventId { get; set; }
            public decimal LaborCost { get; set; }
            public decimal PartsCost { get; set; }
        }
    }
}
