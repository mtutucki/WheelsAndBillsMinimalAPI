using static WheelsAndBillsAPI.Endpoints.Events.EventsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Events.FuelingEvent
{
    public static class FuelingEventsEndpoints
    {
        public static RouteHandlerBuilder MapGetFuelingEvents(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var events = await db.FuelingEvents
                    .Select(e => new GetFuelingEventDTO(
                        e.Id,
                        e.VehicleEventId,
                        e.Liters,
                        e.TotalPrice
                    ))
                    .ToListAsync();

                return Results.Ok(events);
            });
        }

        public static RouteHandlerBuilder MapGetFuelingEventById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var ev = await db.FuelingEvents
                    .Where(e => e.Id == id)
                    .Select(e => new GetFuelingEventDTO(
                        e.Id,
                        e.VehicleEventId,
                        e.Liters,
                        e.TotalPrice
                    ))
                    .FirstOrDefaultAsync();

                return ev is null
                    ? Results.NotFound()
                    : Results.Ok(ev);
            });
        }

        public static RouteHandlerBuilder MapCreateFuelingEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateFuelingEventDTO request,
                AppDbContext db) =>
            {
                var vehicleEventExists = await db.VehicleEvents
                    .AnyAsync(v => v.Id == request.VehicleEventId);

                if (!vehicleEventExists)
                    return Results.BadRequest("VehicleEvent does not exist");

                var ev = new Domain.Entities.Events.FuelingEvent
                {
                    Id = Guid.NewGuid(),
                    VehicleEventId = request.VehicleEventId,
                    Liters = request.Liters,
                    TotalPrice = request.TotalPrice
                };

                db.FuelingEvents.Add(ev);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/fueling-events/{ev.Id}",
                    new GetFuelingEventDTO(
                        ev.Id,
                        ev.VehicleEventId,
                        ev.Liters,
                        ev.TotalPrice
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateFuelingEvent(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateFuelingEventDTO request,
                AppDbContext db) =>
            {
                var ev = await db.FuelingEvents.FindAsync(id);
                if (ev is null)
                    return Results.NotFound();

                ev.Liters = request.Liters;
                ev.TotalPrice = request.TotalPrice;

                await db.SaveChangesAsync();

                return Results.Ok(new GetFuelingEventDTO(
                    ev.Id,
                    ev.VehicleEventId,
                    ev.Liters,
                    ev.TotalPrice
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteFuelingEvent(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var ev = await db.FuelingEvents.FindAsync(id);
                if (ev is null)
                    return Results.NotFound();

                db.FuelingEvents.Remove(ev);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
