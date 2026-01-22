using static WheelsAndBillsAPI.Endpoints.Events.EventsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Events.EventType
{
    public static class EventTypesEndpoints
    {

        public static RouteHandlerBuilder MapGetEventTypes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var types = await db.EventTypes
                    .OrderBy(t => t.Name)
                    .Select(t => new GetEventTypeDTO(
                        t.Id,
                        t.Name
                    ))
                    .ToListAsync();

                return Results.Ok(types);
            });
        }

        public static RouteHandlerBuilder MapGetEventTypeById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var type = await db.EventTypes
                    .Where(t => t.Id == id)
                    .Select(t => new GetEventTypeDTO(
                        t.Id,
                        t.Name
                    ))
                    .FirstOrDefaultAsync();

                return type is null
                    ? Results.NotFound()
                    : Results.Ok(type);
            });
        }


        public static RouteHandlerBuilder MapCreateEventType(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateEventTypeDTO request,
                AppDbContext db) =>
            {
                var exists = await db.EventTypes
                    .AnyAsync(t => t.Name == request.Name);

                if (exists)
                    return Results.BadRequest("Event type already exists");

                var type = new Domain.Entities.Events.EventType
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };

                db.EventTypes.Add(type);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/event-types/{type.Id}",
                    new GetEventTypeDTO(type.Id, type.Name)
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateEventType(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateEventTypeDTO request,
                AppDbContext db) =>
            {
                var type = await db.EventTypes.FindAsync(id);
                if (type is null)
                    return Results.NotFound();

                var exists = await db.EventTypes
                    .AnyAsync(t => t.Name == request.Name && t.Id != id);

                if (exists)
                    return Results.BadRequest("Event type already exists");

                type.Name = request.Name;
                await db.SaveChangesAsync();

                return Results.Ok(new GetEventTypeDTO(
                    type.Id,
                    type.Name
                ));
            });
        }


        public static RouteHandlerBuilder MapDeleteEventType(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var type = await db.EventTypes.FindAsync(id);
                if (type is null)
                    return Results.NotFound();

                db.EventTypes.Remove(type);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
