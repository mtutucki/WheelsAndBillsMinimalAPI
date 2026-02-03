using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using WheelsAndBills.Application.Features.Events.ServiceEvents;

namespace WheelsAndBills.API.Endpoints.Events.ServiceEvents
{
    public static class ServiceEventsEndpoints
    {
        public static RouteHandlerBuilder MapGetServiceEvents(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IServiceEventsService serviceEvents,
                CancellationToken cancellationToken) =>
            {
                var events = await serviceEvents.GetAllAsync(cancellationToken);

                return Results.Ok(events);
            });
        }

        public static RouteHandlerBuilder MapGetServiceEventById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IServiceEventsService serviceEvents,
                CancellationToken cancellationToken) =>
            {
                var ev = await serviceEvents.GetByIdAsync(id, cancellationToken);

                return ev is null
                    ? Results.NotFound()
                    : Results.Ok(ev);
            });
        }

        public static RouteHandlerBuilder MapCreateServiceEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateServiceEventDTO request,
                IServiceEventsService serviceEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await serviceEvents.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/service-events/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateServiceEvent(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateServiceEventDTO request,
                IServiceEventsService serviceEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await serviceEvents.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteServiceEvent(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IServiceEventsService serviceEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await serviceEvents.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
