using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using WheelsAndBills.Application.Features.Events.FuelingEvents;

namespace WheelsAndBills.API.Endpoints.Events.FuelingEvent
{
    public static class FuelingEventsEndpoints
    {
        public static RouteHandlerBuilder MapGetFuelingEvents(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IFuelingEventsService fuelingEvents,
                CancellationToken cancellationToken) =>
            {
                var events = await fuelingEvents.GetAllAsync(cancellationToken);

                return Results.Ok(events);
            });
        }

        public static RouteHandlerBuilder MapGetFuelingEventById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IFuelingEventsService fuelingEvents,
                CancellationToken cancellationToken) =>
            {
                var ev = await fuelingEvents.GetByIdAsync(id, cancellationToken);

                return ev is null
                    ? Results.NotFound()
                    : Results.Ok(ev);
            });
        }

        public static RouteHandlerBuilder MapCreateFuelingEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateFuelingEventDTO request,
                IFuelingEventsService fuelingEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await fuelingEvents.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/fueling-events/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateFuelingEvent(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateFuelingEventDTO request,
                IFuelingEventsService fuelingEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await fuelingEvents.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteFuelingEvent(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IFuelingEventsService fuelingEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await fuelingEvents.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
