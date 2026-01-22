using static WheelsAndBillsAPI.Endpoints.Reports.ReportDTOs;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Reports.Reports
{
    public static class ReportsEndpoints
    {
        public static RouteHandlerBuilder MapGetReports(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var reports = await db.Reports
                    .Select(r => new GetReportDTO(
                        r.Id,
                        r.UserId,
                        r.ReportDefinitionId,
                        r.CreatedAt
                    ))
                    .ToListAsync();

                return Results.Ok(reports);
            });
        }

        public static RouteHandlerBuilder MapGetReportById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var report = await db.Reports
                    .Where(r => r.Id == id)
                    .Select(r => new GetReportDTO(
                        r.Id,
                        r.UserId,
                        r.ReportDefinitionId,
                        r.CreatedAt
                    ))
                    .FirstOrDefaultAsync();

                return report is null
                    ? Results.NotFound()
                    : Results.Ok(report);
            });
        }

        public static RouteHandlerBuilder MapCreateReport(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateReportDTO request,
                AppDbContext db) =>
            {
                var userExists = await db.Users.AnyAsync(u => u.Id == request.UserId);
                if (!userExists)
                    return Results.BadRequest("User does not exist");

                var definitionExists = await db.ReportDefinitions
                    .AnyAsync(d => d.Id == request.ReportDefinitionId);
                if (!definitionExists)
                    return Results.BadRequest("ReportDefinition does not exist");

                var report = new Domain.Entities.Report.Report
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    ReportDefinitionId = request.ReportDefinitionId,
                    CreatedAt = DateTime.UtcNow
                };

                db.Reports.Add(report);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/reports/{report.Id}",
                    new GetReportDTO(
                        report.Id,
                        report.UserId,
                        report.ReportDefinitionId,
                        report.CreatedAt
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateReport(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateReportDTO request,
                AppDbContext db) =>
            {
                var report = await db.Reports.FindAsync(id);
                if (report is null)
                    return Results.NotFound();

                var definitionExists = await db.ReportDefinitions
                    .AnyAsync(d => d.Id == request.ReportDefinitionId);
                if (!definitionExists)
                    return Results.BadRequest("ReportDefinition does not exist");

                report.ReportDefinitionId = request.ReportDefinitionId;
                await db.SaveChangesAsync();

                return Results.Ok(new GetReportDTO(
                    report.Id,
                    report.UserId,
                    report.ReportDefinitionId,
                    report.CreatedAt
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteReport(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var report = await db.Reports.FindAsync(id);
                if (report is null)
                    return Results.NotFound();

                db.Reports.Remove(report);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
