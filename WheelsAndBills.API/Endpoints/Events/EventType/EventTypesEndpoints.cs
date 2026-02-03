using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using WheelsAndBills.Application.Features.Events.EventTypes;

namespace WheelsAndBills.API.Endpoints.Events.EventType
{
    public static class EventTypesEndpoints
    {

        public static RouteHandlerBuilder MapGetEventTypes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IEventTypesService eventTypes,
                CancellationToken cancellationToken) =>
            {
                var types = await eventTypes.GetAllAsync(cancellationToken);

                return Results.Ok(types);
            });
        }

        public static RouteHandlerBuilder MapGetEventTypeById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IEventTypesService eventTypes,
                CancellationToken cancellationToken) =>
            {
                var type = await eventTypes.GetByIdAsync(id, cancellationToken);

                return type is null
                    ? Results.NotFound()
                    : Results.Ok(type);
            });
        }


        public static RouteHandlerBuilder MapCreateEventType(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateEventTypeDTO request,
                IEventTypesService eventTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await eventTypes.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/event-types/{result.Data!.Id}",
                    result.Data
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateEventType(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateEventTypeDTO request,
                IEventTypesService eventTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await eventTypes.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }


        public static RouteHandlerBuilder MapDeleteEventType(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IEventTypesService eventTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await eventTypes.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
