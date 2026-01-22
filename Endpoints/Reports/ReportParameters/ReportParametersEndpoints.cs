using static WheelsAndBillsAPI.Endpoints.Reports.ReportDTOs;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Reports.ReportParameters
{
    public static class ReportParametersEndpoints
    {
        public static RouteHandlerBuilder MapGetReportParameters(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var items = await db.ReportParameters
                    .Select(p => new GetReportParameterDTO(
                        p.Id,
                        p.ReportId,
                        p.Name,
                        p.Value
                    ))
                    .ToListAsync();

                return Results.Ok(items);
            });
        }

        public static RouteHandlerBuilder MapGetReportParameterById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var item = await db.ReportParameters
                    .Where(p => p.Id == id)
                    .Select(p => new GetReportParameterDTO(
                        p.Id,
                        p.ReportId,
                        p.Name,
                        p.Value
                    ))
                    .FirstOrDefaultAsync();

                return item is null
                    ? Results.NotFound()
                    : Results.Ok(item);
            });
        }

        public static RouteHandlerBuilder MapCreateReportParameter(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateReportParameterDTO request,
                AppDbContext db) =>
            {
                var reportExists = await db.Reports
                    .AnyAsync(r => r.Id == request.ReportId);
                if (!reportExists)
                    return Results.BadRequest("Report does not exist");

                var param = new Domain.Entities.Report.ReportParameter
                {
                    Id = Guid.NewGuid(),
                    ReportId = request.ReportId,
                    Name = request.Name,
                    Value = request.Value
                };

                db.ReportParameters.Add(param);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/report-parameters/{param.Id}",
                    new GetReportParameterDTO(
                        param.Id,
                        param.ReportId,
                        param.Name,
                        param.Value
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateReportParameter(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateReportParameterDTO request,
                AppDbContext db) =>
            {
                var param = await db.ReportParameters.FindAsync(id);
                if (param is null)
                    return Results.NotFound();

                param.Name = request.Name;
                param.Value = request.Value;

                await db.SaveChangesAsync();

                return Results.Ok(new GetReportParameterDTO(
                    param.Id,
                    param.ReportId,
                    param.Name,
                    param.Value
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteReportParameter(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var param = await db.ReportParameters.FindAsync(id);
                if (param is null)
                    return Results.NotFound();

                db.ReportParameters.Remove(param);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
