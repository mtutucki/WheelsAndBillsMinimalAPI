using static WheelsAndBillsAPI.Endpoints.Events.EventsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Events;

namespace WheelsAndBillsAPI.Endpoints.Events.Parts
{
    public static class PartsEndpoints
    {
        public static RouteHandlerBuilder MapGetParts(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var parts = await db.Parts
                    .OrderBy(p => p.Name)
                    .Select(p => new GetPartDTO(p.Id, p.Name))
                    .ToListAsync();

                return Results.Ok(parts);
            });
        }

        public static RouteHandlerBuilder MapGetPartById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var part = await db.Parts
                    .Where(p => p.Id == id)
                    .Select(p => new GetPartDTO(p.Id, p.Name))
                    .FirstOrDefaultAsync();

                return part is null
                    ? Results.NotFound()
                    : Results.Ok(part);
            });
        }

        public static RouteHandlerBuilder MapCreatePart(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreatePartDTO request,
                AppDbContext db) =>
            {
                var exists = await db.Parts.AnyAsync(p => p.Name == request.Name);
                if (exists)
                    return Results.BadRequest("Part already exists");

                var part = new Part
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };

                db.Parts.Add(part);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/parts/{part.Id}",
                    new GetPartDTO(part.Id, part.Name)
                );
            });
        }

        public static RouteHandlerBuilder MapUpdatePart(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdatePartDTO request,
                AppDbContext db) =>
            {
                var part = await db.Parts.FindAsync(id);
                if (part is null)
                    return Results.NotFound();

                var exists = await db.Parts.AnyAsync(p => p.Name == request.Name && p.Id != id);
                if (exists)
                    return Results.BadRequest("Part already exists");

                part.Name = request.Name;
                await db.SaveChangesAsync();

                return Results.Ok(new GetPartDTO(part.Id, part.Name));
            });
        }

        public static RouteHandlerBuilder MapDeletePart(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var part = await db.Parts.FindAsync(id);
                if (part is null)
                    return Results.NotFound();

                db.Parts.Remove(part);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
