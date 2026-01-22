using static WheelsAndBillsAPI.Endpoints.Events.EventsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Events.Workshops
{
    public static class WorkshopsEndpoints
    {
        public static RouteHandlerBuilder MapGetWorkshops(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var workshops = await db.Workshops
                    .OrderBy(w => w.Name)
                    .Select(w => new GetWorkshopDTO(w.Id, w.Name))
                    .ToListAsync();

                return Results.Ok(workshops);
            });
        }

        public static RouteHandlerBuilder MapGetWorkshopById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var workshop = await db.Workshops
                    .Where(w => w.Id == id)
                    .Select(w => new GetWorkshopDTO(w.Id, w.Name))
                    .FirstOrDefaultAsync();

                return workshop is null
                    ? Results.NotFound()
                    : Results.Ok(workshop);
            });
        }

        public static RouteHandlerBuilder MapCreateWorkshop(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateWorkshopDTO request,
                AppDbContext db) =>
            {
                var exists = await db.Workshops.AnyAsync(w => w.Name == request.Name);
                if (exists)
                    return Results.BadRequest("Workshop already exists");

                var workshop = new Domain.Entities.Events.Workshop
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };

                db.Workshops.Add(workshop);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/workshops/{workshop.Id}",
                    new GetWorkshopDTO(workshop.Id, workshop.Name)
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateWorkshop(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateWorkshopDTO request,
                AppDbContext db) =>
            {
                var workshop = await db.Workshops.FindAsync(id);
                if (workshop is null)
                    return Results.NotFound();

                var exists = await db.Workshops.AnyAsync(w => w.Name == request.Name && w.Id != id);
                if (exists)
                    return Results.BadRequest("Workshop already exists");

                workshop.Name = request.Name;
                await db.SaveChangesAsync();

                return Results.Ok(new GetWorkshopDTO(workshop.Id, workshop.Name));
            });
        }

        public static RouteHandlerBuilder MapDeleteWorkshop(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var workshop = await db.Workshops.FindAsync(id);
                if (workshop is null)
                    return Results.NotFound();

                db.Workshops.Remove(workshop);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
