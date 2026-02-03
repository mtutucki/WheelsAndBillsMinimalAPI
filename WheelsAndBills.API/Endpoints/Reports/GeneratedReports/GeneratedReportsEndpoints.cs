using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;
using WheelsAndBills.Application.Features.Reports.GeneratedReports;

namespace WheelsAndBills.API.Endpoints.Reports.GeneratedReports
{
    public static class GeneratedReportsEndpoints
    {
        public static RouteHandlerBuilder MapGetGeneratedReports(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IGeneratedReportsService generatedReports,
                CancellationToken cancellationToken) =>
            {
                var reports = await generatedReports.GetAllAsync(cancellationToken);

                return Results.Ok(reports);
            });
        }

        public static RouteHandlerBuilder MapGetGeneratedReportById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IGeneratedReportsService generatedReports,
                CancellationToken cancellationToken) =>
            {
                var report = await generatedReports.GetByIdAsync(id, cancellationToken);

                return report is null
                    ? Results.NotFound()
                    : Results.Ok(report);
            });
        }

        public static RouteHandlerBuilder MapCreateGeneratedReport(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateGeneratedReportDTO request,
                IGeneratedReportsService generatedReports,
                CancellationToken cancellationToken) =>
            {
                var result = await generatedReports.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/generated-reports/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateGeneratedReport(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateGeneratedReportDTO request,
                IGeneratedReportsService generatedReports,
                CancellationToken cancellationToken) =>
            {
                var result = await generatedReports.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteGeneratedReport(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IGeneratedReportsService generatedReports,
                CancellationToken cancellationToken) =>
            {
                var result = await generatedReports.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
