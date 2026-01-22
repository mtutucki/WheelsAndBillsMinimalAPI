using static WheelsAndBillsAPI.Endpoints.Reports.ReportDTOs;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Reports.ReportDefinitions
{
    public static class ReportDefinitionsEndpoints
    {
        public static RouteHandlerBuilder MapGetReportDefinitions(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var defs = await db.ReportDefinitions
                    .OrderBy(d => d.Code)
                    .Select(d => new GetReportDefinitionDTO(d.Id, d.Code))
                    .ToListAsync();

                return Results.Ok(defs);
            });
        }

        public static RouteHandlerBuilder MapGetReportDefinitionById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var def = await db.ReportDefinitions
                    .Where(d => d.Id == id)
                    .Select(d => new GetReportDefinitionDTO(d.Id, d.Code))
                    .FirstOrDefaultAsync();

                return def is null
                    ? Results.NotFound()
                    : Results.Ok(def);
            });
        }

        public static RouteHandlerBuilder MapCreateReportDefinition(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateReportDefinitionDTO request,
                AppDbContext db) =>
            {
                var exists = await db.ReportDefinitions
                    .AnyAsync(d => d.Code == request.Code);

                if (exists)
                    return Results.BadRequest("ReportDefinition already exists");

                var def = new Domain.Entities.Report.ReportDefinition
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code
                };

                db.ReportDefinitions.Add(def);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/report-definitions/{def.Id}",
                    new GetReportDefinitionDTO(def.Id, def.Code)
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateReportDefinition(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateReportDefinitionDTO request,
                AppDbContext db) =>
            {
                var def = await db.ReportDefinitions.FindAsync(id);
                if (def is null)
                    return Results.NotFound();

                var exists = await db.ReportDefinitions
                    .AnyAsync(d => d.Code == request.Code && d.Id != id);

                if (exists)
                    return Results.BadRequest("ReportDefinition already exists");

                def.Code = request.Code;
                await db.SaveChangesAsync();

                return Results.Ok(new GetReportDefinitionDTO(def.Id, def.Code));
            });
        }

        public static RouteHandlerBuilder MapDeleteReportDefinition(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var def = await db.ReportDefinitions.FindAsync(id);
                if (def is null)
                    return Results.NotFound();

                db.ReportDefinitions.Remove(def);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
