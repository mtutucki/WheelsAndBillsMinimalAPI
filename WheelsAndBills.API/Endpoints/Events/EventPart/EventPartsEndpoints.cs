using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using WheelsAndBills.Application.Features.Events.EventParts;

namespace WheelsAndBills.API.Endpoints.Events.EventPart
{
    public static class EventPartsEndpoints
    {

        public static RouteHandlerBuilder MapGetEventPartsByRepairEvent(this RouteGroupBuilder app)
        {
            return app.MapGet("/{repairEventId:guid}", async (
                Guid repairEventId,
                IEventPartsService eventParts,
                CancellationToken cancellationToken) =>
            {
                var parts = await eventParts.GetByRepairEventAsync(repairEventId, cancellationToken);

                return Results.Ok(parts);
            });
        }


        public static RouteHandlerBuilder MapCreateEventPart(this RouteGroupBuilder app)
        {
            return app.MapPost("/{repairEventId:guid}", async (
                Guid repairEventId,
                CreateEventPartDTO request,
                IEventPartsService eventParts,
                CancellationToken cancellationToken) =>
            {
                var result = await eventParts.CreateAsync(repairEventId, request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/repair-events/{result.Data!.RepairEventId}/parts/{result.Data.PartId}",
                    result.Data
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateEventPart(this RouteGroupBuilder app)
        {
            return app.MapPut("/{repairEventId:guid}/{partId:guid}", async (
                Guid repairEventId,
                Guid partId,
                UpdateEventPartDTO request,
                IEventPartsService eventParts,
                CancellationToken cancellationToken) =>
            {
                var result = await eventParts.UpdateAsync(repairEventId, partId, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }


        public static RouteHandlerBuilder MapDeleteEventPart(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{repairEventId:guid}/{partId:guid}", async (
                Guid repairEventId,
                Guid partId,
                IEventPartsService eventParts,
                CancellationToken cancellationToken) =>
            {
                var result = await eventParts.DeleteAsync(repairEventId, partId, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
