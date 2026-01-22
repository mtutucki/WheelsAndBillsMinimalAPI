using static WheelsAndBillsAPI.Endpoints.Events.EventsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Events.VehicleEvents
{
    public static class VehicleEventsEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleEvents(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var events = await db.VehicleEvents
                    .Select(e => new GetVehicleEventDTO(
                        e.Id,
                        e.VehicleId,
                        e.EventTypeId,
                        e.EventDate,
                        e.Mileage,
                        e.Description
                    ))
                    .ToListAsync();

                return Results.Ok(events);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleEventById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var ev = await db.VehicleEvents
                    .Where(e => e.Id == id)
                    .Select(e => new GetVehicleEventDTO(
                        e.Id,
                        e.VehicleId,
                        e.EventTypeId,
                        e.EventDate,
                        e.Mileage,
                        e.Description
                    ))
                    .FirstOrDefaultAsync();

                return ev is null
                    ? Results.NotFound()
                    : Results.Ok(ev);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleEventDTO request,
                AppDbContext db) =>
            {
                var vehicleExists = await db.Vehicles
                    .AnyAsync(v => v.Id == request.VehicleId);
                if (!vehicleExists)
                    return Results.BadRequest("Vehicle does not exist");

                var eventTypeExists = await db.EventTypes
                    .AnyAsync(et => et.Id == request.EventTypeId);
                if (!eventTypeExists)
                    return Results.BadRequest("EventType does not exist");

                var ev = new Domain.Entities.Events.VehicleEvent
                {
                    Id = Guid.NewGuid(),
                    VehicleId = request.VehicleId,
                    EventTypeId = request.EventTypeId,
                    EventDate = request.EventDate,
                    Mileage = request.Mileage,
                    Description = request.Description
                };

                db.VehicleEvents.Add(ev);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicle-events/{ev.Id}",
                    new GetVehicleEventDTO(
                        ev.Id,
                        ev.VehicleId,
                        ev.EventTypeId,
                        ev.EventDate,
                        ev.Mileage,
                        ev.Description
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleEvent(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleEventDTO request,
                AppDbContext db) =>
            {
                var ev = await db.VehicleEvents.FindAsync(id);
                if (ev is null)
                    return Results.NotFound();

                var eventTypeExists = await db.EventTypes
                    .AnyAsync(et => et.Id == request.EventTypeId);
                if (!eventTypeExists)
                    return Results.BadRequest("EventType does not exist");

                ev.EventTypeId = request.EventTypeId;
                ev.EventDate = request.EventDate;
                ev.Mileage = request.Mileage;
                ev.Description = request.Description;

                await db.SaveChangesAsync();

                return Results.Ok(new GetVehicleEventDTO(
                    ev.Id,
                    ev.VehicleId,
                    ev.EventTypeId,
                    ev.EventDate,
                    ev.Mileage,
                    ev.Description
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleEvent(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var ev = await db.VehicleEvents.FindAsync(id);
                if (ev is null)
                    return Results.NotFound();

                db.VehicleEvents.Remove(ev);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
