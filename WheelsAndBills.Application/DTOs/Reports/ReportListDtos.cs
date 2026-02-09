namespace WheelsAndBills.Application.DTOs.Reports
{
    public record ReportListItemDto(
        Guid Id,
        string DefinitionCode,
        DateTime CreatedAt,
        IReadOnlyList<ReportParameterItemDto> Parameters
    );

    public record ReportDetailDto(
        Guid Id,
        string DefinitionCode,
        DateTime CreatedAt,
        IReadOnlyList<ReportParameterItemDto> Parameters,
        IReadOnlyList<GeneratedReportItemDto> GeneratedReports
    );

    public record ReportParameterItemDto(
        string Name,
        string Value
    );

    public record GeneratedReportItemDto(
        Guid Id,
        string FilePath
    );
}
