using static WheelsAndBillsAPI.Endpoints.Events.EventsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Events.RepairEvents
{
    public static class RepairEventsEndpoints
    {
        public static RouteHandlerBuilder MapGetRepairEvents(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var events = await db.RepairEvents
                    .Select(e => new GetRepairEventDTO(
                        e.Id,
                        e.VehicleEventId,
                        e.LaborCost
                    ))
                    .ToListAsync();

                return Results.Ok(events);
            });
        }

        public static RouteHandlerBuilder MapGetRepairEventById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var ev = await db.RepairEvents
                    .Where(e => e.Id == id)
                    .Select(e => new GetRepairEventDTO(
                        e.Id,
                        e.VehicleEventId,
                        e.LaborCost
                    ))
                    .FirstOrDefaultAsync();

                return ev is null
                    ? Results.NotFound()
                    : Results.Ok(ev);
            });
        }

        public static RouteHandlerBuilder MapCreateRepairEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateRepairEventDTO request,
                AppDbContext db) =>
            {
                var vehicleEventExists = await db.VehicleEvents
                    .AnyAsync(v => v.Id == request.VehicleEventId);

                if (!vehicleEventExists)
                    return Results.BadRequest("VehicleEvent does not exist");

                var ev = new Domain.Entities.Events.RepairEvent
                {
                    Id = Guid.NewGuid(),
                    VehicleEventId = request.VehicleEventId,
                    LaborCost = request.LaborCost
                };

                db.RepairEvents.Add(ev);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/repair-events/{ev.Id}",
                    new GetRepairEventDTO(
                        ev.Id,
                        ev.VehicleEventId,
                        ev.LaborCost
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateRepairEvent(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateRepairEventDTO request,
                AppDbContext db) =>
            {
                var ev = await db.RepairEvents.FindAsync(id);
                if (ev is null)
                    return Results.NotFound();

                ev.LaborCost = request.LaborCost;
                await db.SaveChangesAsync();

                return Results.Ok(new GetRepairEventDTO(
                    ev.Id,
                    ev.VehicleEventId,
                    ev.LaborCost
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteRepairEvent(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var ev = await db.RepairEvents.FindAsync(id);
                if (ev is null)
                    return Results.NotFound();

                db.RepairEvents.Remove(ev);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
