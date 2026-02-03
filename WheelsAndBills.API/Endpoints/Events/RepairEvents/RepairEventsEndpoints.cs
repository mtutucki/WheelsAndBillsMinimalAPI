using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using WheelsAndBills.Application.Features.Events.RepairEvents;

namespace WheelsAndBills.API.Endpoints.Events.RepairEvents
{
    public static class RepairEventsEndpoints
    {
        public static RouteHandlerBuilder MapGetRepairEvents(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IRepairEventsService repairEvents,
                CancellationToken cancellationToken) =>
            {
                var events = await repairEvents.GetAllAsync(cancellationToken);

                return Results.Ok(events);
            });
        }

        public static RouteHandlerBuilder MapGetRepairEventById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IRepairEventsService repairEvents,
                CancellationToken cancellationToken) =>
            {
                var ev = await repairEvents.GetByIdAsync(id, cancellationToken);

                return ev is null
                    ? Results.NotFound()
                    : Results.Ok(ev);
            });
        }

        public static RouteHandlerBuilder MapCreateRepairEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateRepairEventDTO request,
                IRepairEventsService repairEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await repairEvents.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/repair-events/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateRepairEvent(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateRepairEventDTO request,
                IRepairEventsService repairEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await repairEvents.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteRepairEvent(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IRepairEventsService repairEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await repairEvents.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
