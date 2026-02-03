using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;
using WheelsAndBills.Application.Features.Reports.Reports;

namespace WheelsAndBills.API.Endpoints.Reports.Reports
{
    public static class ReportsEndpoints
    {
        public static RouteHandlerBuilder MapGetReports(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var reports = await reportsService.GetAllAsync(cancellationToken);

                return Results.Ok(reports);
            });
        }

        public static RouteHandlerBuilder MapGetReportById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var report = await reportsService.GetByIdAsync(id, cancellationToken);

                return report is null
                    ? Results.NotFound()
                    : Results.Ok(report);
            });
        }

        public static RouteHandlerBuilder MapCreateReport(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateReportDTO request,
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportsService.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/reports/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateReport(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateReportDTO request,
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportsService.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteReport(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportsService.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
