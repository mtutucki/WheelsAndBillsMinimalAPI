using static WheelsAndBillsAPI.Endpoints.Events.EventsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WheelsAndBillsAPI.Domain.Entities.Vehicles;
using WheelsAndBillsAPI.Helpers.Events;

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

        public static RouteHandlerBuilder MapCreateMyVehicleEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("/my-events", async (
                CreateMyVehicleEventDTO request,
                ClaimsPrincipal user,
                AppDbContext db) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var vehicle = await db.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == request.VehicleId && v.UserId == userId);

                if (vehicle is null)
                    return Results.BadRequest("Vehicle does not belong to user");

                var eventTypeExists = await db.EventTypes
                    .AnyAsync(et => et.Id == request.EventTypeId);

                if (!eventTypeExists)
                    return Results.BadRequest("EventType does not exist");

                int lastMileage = 0;
                if (request.Mileage > 0)
                {
                    lastMileage = await db.VehicleMileage
                        .Where(m => m.VehicleId == request.VehicleId)
                        .OrderByDescending(m => m.Mileage)
                        .Select(m => m.Mileage)
                        .FirstOrDefaultAsync();

                    if (request.Mileage < lastMileage)
                    {
                        return Results.BadRequest(
                            $"Mileage cannot be lower than last recorded mileage ({lastMileage})"
                        );
                    }
                }

                using var tx = await db.Database.BeginTransactionAsync();

                var ev = new Domain.Entities.Events.VehicleEvent
                {
                    Id = Guid.NewGuid(),
                    VehicleId = request.VehicleId,
                    EventTypeId = request.EventTypeId,
                    EventDate = request.EventDate.Date,
                    Mileage = request.Mileage,
                    Description = request.Description,
                    CreatedAt = DateTime.Now,
                };

               
                if (request.Mileage > 0 && request.Mileage > lastMileage)
                {
                    db.VehicleMileage.Add(new VehicleMileage
                    {
                        Id = Guid.NewGuid(),
                        VehicleId = request.VehicleId,
                        Mileage = request.Mileage,
                        Date = request.EventDate.Date
                    });

                }

                db.VehicleEvents.Add(ev);
                await db.SaveChangesAsync();

                await VehicleEventDetailsHandler.Handle(request, ev.Id, db);

                await db.SaveChangesAsync();
                await tx.CommitAsync();
                
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
    }
}
