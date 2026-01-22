using static WheelsAndBillsAPI.Endpoints.Reports.ReportDTOs;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Reports.GeneratedReports
{
    public static class GeneratedReportsEndpoints
    {
        public static RouteHandlerBuilder MapGetGeneratedReports(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var reports = await db.GeneratedReports
                    .Select(r => new GetGeneratedReportDTO(
                        r.Id,
                        r.ReportId,
                        r.FilePath
                    ))
                    .ToListAsync();

                return Results.Ok(reports);
            });
        }

        public static RouteHandlerBuilder MapGetGeneratedReportById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var report = await db.GeneratedReports
                    .Where(r => r.Id == id)
                    .Select(r => new GetGeneratedReportDTO(
                        r.Id,
                        r.ReportId,
                        r.FilePath
                    ))
                    .FirstOrDefaultAsync();

                return report is null
                    ? Results.NotFound()
                    : Results.Ok(report);
            });
        }

        public static RouteHandlerBuilder MapCreateGeneratedReport(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateGeneratedReportDTO request,
                AppDbContext db) =>
            {
                var reportExists = await db.Reports
                    .AnyAsync(r => r.Id == request.ReportId);
                if (!reportExists)
                    return Results.BadRequest("Report does not exist");

                var generated = new Domain.Entities.Report.GeneratedReport
                {
                    Id = Guid.NewGuid(),
                    ReportId = request.ReportId,
                    FilePath = request.FilePath
                };

                db.GeneratedReports.Add(generated);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/generated-reports/{generated.Id}",
                    new GetGeneratedReportDTO(
                        generated.Id,
                        generated.ReportId,
                        generated.FilePath
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateGeneratedReport(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateGeneratedReportDTO request,
                AppDbContext db) =>
            {
                var generated = await db.GeneratedReports.FindAsync(id);
                if (generated is null)
                    return Results.NotFound();

                generated.FilePath = request.FilePath;
                await db.SaveChangesAsync();

                return Results.Ok(new GetGeneratedReportDTO(
                    generated.Id,
                    generated.ReportId,
                    generated.FilePath
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteGeneratedReport(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var generated = await db.GeneratedReports.FindAsync(id);
                if (generated is null)
                    return Results.NotFound();

                db.GeneratedReports.Remove(generated);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
