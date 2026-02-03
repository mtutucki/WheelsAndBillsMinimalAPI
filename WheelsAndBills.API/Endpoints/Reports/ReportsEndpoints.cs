using WheelsAndBills.API.Endpoints.Reports.GeneratedReports;
using WheelsAndBills.API.Endpoints.Reports.ReportDefinitions;
using WheelsAndBills.API.Endpoints.Reports.ReportParameters;
using WheelsAndBills.API.Endpoints.Reports.Reports;

namespace WheelsAndBills.API.Endpoints.Reports
{
    public static class ReportsEndpoints
    {
        public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
        {
            var generatedReports = app
                .MapGroup("/reports-generated")
                .WithTags("Generated reports")
                .RequireAuthorization();

            var reports = app
                .MapGroup("/reports")
                .WithTags("Reports")
                .RequireAuthorization();

            var reportsDefinitions = app
                .MapGroup("/reports-defitnion")
                .WithTags("Reports definitions")
                .RequireAuthorization();

            var reportsParameters = app
                .MapGroup("/reports-parameters")
                .WithTags("Reports parameters")
                .RequireAuthorization();

            generatedReports.MapCreateGeneratedReport();
            generatedReports.MapUpdateGeneratedReport();
            generatedReports.MapDeleteGeneratedReport();
            generatedReports.MapGetGeneratedReports();
            generatedReports.MapGetGeneratedReportById();


            reports.MapCreateReport();
            reports.MapUpdateReport();
            reports.MapDeleteReport();
            reports.MapGetReports();
            reports.MapGetReportById();

            reportsDefinitions.MapCreateReportDefinition();
            reportsDefinitions.MapUpdateReportDefinition();
            reportsDefinitions.MapDeleteReportDefinition();
            reportsDefinitions.MapGetReportDefinitions();
            reportsDefinitions.MapGetReportDefinitionById();

            reportsParameters.MapCreateReportParameter();
            reportsParameters.MapUpdateReportParameter();
            reportsParameters.MapDeleteReportParameter();
            reportsParameters.MapGetReportParameters();
            reportsParameters.MapGetReportParameterById();

            return app;
        }
    }
}
