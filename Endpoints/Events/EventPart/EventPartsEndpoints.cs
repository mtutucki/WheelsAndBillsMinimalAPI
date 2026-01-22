using static WheelsAndBillsAPI.Endpoints.Events.EventsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Events.EventPart
{
    public static class EventPartsEndpoints
    {

        public static RouteHandlerBuilder MapGetEventPartsByRepairEvent(this RouteGroupBuilder app)
        {
            return app.MapGet("/{repairEventId:guid}", async (
                Guid repairEventId,
                AppDbContext db) =>
            {
                var parts = await db.EventParts
                    .Where(ep => ep.RepairEventId == repairEventId)
                    .Select(ep => new GetEventPartDTO(
                        ep.RepairEventId,
                        ep.PartId,
                        ep.Price
                    ))
                    .ToListAsync();

                return Results.Ok(parts);
            });
        }


        public static RouteHandlerBuilder MapCreateEventPart(this RouteGroupBuilder app)
        {
            return app.MapPost("/{repairEventId:guid}", async (
                Guid repairEventId,
                CreateEventPartDTO request,
                AppDbContext db) =>
            {
                if (repairEventId != request.RepairEventId)
                    return Results.BadRequest("RepairEventId mismatch");

                var repairEventExists = await db.RepairEvents
                    .AnyAsync(e => e.Id == request.RepairEventId);
                if (!repairEventExists)
                    return Results.BadRequest("RepairEvent does not exist");

                var partExists = await db.Parts
                    .AnyAsync(p => p.Id == request.PartId);
                if (!partExists)
                    return Results.BadRequest("Part does not exist");

                var exists = await db.EventParts
                    .AnyAsync(ep =>
                        ep.RepairEventId == request.RepairEventId &&
                        ep.PartId == request.PartId);

                if (exists)
                    return Results.BadRequest("Part already added to repair event");

                var eventPart = new Domain.Entities.Events.EventPart
                {
                    RepairEventId = request.RepairEventId,
                    PartId = request.PartId,
                    Price = request.Price
                };

                db.EventParts.Add(eventPart);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/repair-events/{eventPart.RepairEventId}/parts/{eventPart.PartId}",
                    new GetEventPartDTO(
                        eventPart.RepairEventId,
                        eventPart.PartId,
                        eventPart.Price
                    )
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateEventPart(this RouteGroupBuilder app)
        {
            return app.MapPut("/{repairEventId:guid}/{partId:guid}", async (
                Guid repairEventId,
                Guid partId,
                UpdateEventPartDTO request,
                AppDbContext db) =>
            {
                var eventPart = await db.EventParts
                    .FirstOrDefaultAsync(ep =>
                        ep.RepairEventId == repairEventId &&
                        ep.PartId == partId);

                if (eventPart is null)
                    return Results.NotFound();

                eventPart.Price = request.Price;
                await db.SaveChangesAsync();

                return Results.Ok(new GetEventPartDTO(
                    eventPart.RepairEventId,
                    eventPart.PartId,
                    eventPart.Price
                ));
            });
        }


        public static RouteHandlerBuilder MapDeleteEventPart(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{repairEventId:guid}/{partId:guid}", async (
                Guid repairEventId,
                Guid partId,
                AppDbContext db) =>
            {
                var eventPart = await db.EventParts
                    .FirstOrDefaultAsync(ep =>
                        ep.RepairEventId == repairEventId &&
                        ep.PartId == partId);

                if (eventPart is null)
                    return Results.NotFound();

                db.EventParts.Remove(eventPart);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
