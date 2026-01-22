using static WheelsAndBillsAPI.Endpoints.Events.EventsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Events.ServiceEvents
{
    public static class ServiceEventsEndpoints
    {
        public static RouteHandlerBuilder MapGetServiceEvents(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var events = await db.ServiceEvents
                    .Select(e => new GetServiceEventDTO(
                        e.Id,
                        e.VehicleEventId,
                        e.WorkshopId
                    ))
                    .ToListAsync();

                return Results.Ok(events);
            });
        }

        public static RouteHandlerBuilder MapGetServiceEventById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var ev = await db.ServiceEvents
                    .Where(e => e.Id == id)
                    .Select(e => new GetServiceEventDTO(
                        e.Id,
                        e.VehicleEventId,
                        e.WorkshopId
                    ))
                    .FirstOrDefaultAsync();

                return ev is null
                    ? Results.NotFound()
                    : Results.Ok(ev);
            });
        }

        public static RouteHandlerBuilder MapCreateServiceEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateServiceEventDTO request,
                AppDbContext db) =>
            {
                var vehicleEventExists = await db.VehicleEvents
                    .AnyAsync(v => v.Id == request.VehicleEventId);
                if (!vehicleEventExists)
                    return Results.BadRequest("VehicleEvent does not exist");

                var workshopExists = await db.Workshops
                    .AnyAsync(w => w.Id == request.WorkshopId);
                if (!workshopExists)
                    return Results.BadRequest("Workshop does not exist");

                var ev = new Domain.Entities.Events.ServiceEvent
                {
                    Id = Guid.NewGuid(),
                    VehicleEventId = request.VehicleEventId,
                    WorkshopId = request.WorkshopId
                };

                db.ServiceEvents.Add(ev);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/service-events/{ev.Id}",
                    new GetServiceEventDTO(
                        ev.Id,
                        ev.VehicleEventId,
                        ev.WorkshopId
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateServiceEvent(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateServiceEventDTO request,
                AppDbContext db) =>
            {
                var ev = await db.ServiceEvents.FindAsync(id);
                if (ev is null)
                    return Results.NotFound();

                var workshopExists = await db.Workshops
                    .AnyAsync(w => w.Id == request.WorkshopId);
                if (!workshopExists)
                    return Results.BadRequest("Workshop does not exist");

                ev.WorkshopId = request.WorkshopId;
                await db.SaveChangesAsync();

                return Results.Ok(new GetServiceEventDTO(
                    ev.Id,
                    ev.VehicleEventId,
                    ev.WorkshopId
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteServiceEvent(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var ev = await db.ServiceEvents.FindAsync(id);
                if (ev is null)
                    return Results.NotFound();

                db.ServiceEvents.Remove(ev);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
