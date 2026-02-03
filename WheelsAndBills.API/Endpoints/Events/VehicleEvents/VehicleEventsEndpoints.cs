using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using System.Security.Claims;
using WheelsAndBills.Application.Features.Events.VehicleEvents;

namespace WheelsAndBills.API.Endpoints.Events.VehicleEvents
{
    public static class VehicleEventsEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleEvents(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IVehicleEventsService vehicleEvents,
                CancellationToken cancellationToken) =>
            {
                var events = await vehicleEvents.GetAllAsync(cancellationToken);

                return Results.Ok(events);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleEventById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IVehicleEventsService vehicleEvents,
                CancellationToken cancellationToken) =>
            {
                var ev = await vehicleEvents.GetByIdAsync(id, cancellationToken);

                return ev is null
                    ? Results.NotFound()
                    : Results.Ok(ev);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleEventDTO request,
                IVehicleEventsService vehicleEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleEvents.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-events/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleEvent(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleEventDTO request,
                IVehicleEventsService vehicleEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleEvents.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleEvent(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IVehicleEventsService vehicleEvents,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleEvents.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapDeleteMyVehicleEvent(this RouteGroupBuilder app)
        {
            return app.MapDelete("/my-events/{id:guid}", async (
                Guid id,
                ClaimsPrincipal user,
                IVehicleEventsService vehicleEvents,
                CancellationToken cancellationToken) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var result = await vehicleEvents.DeleteForUserAsync(userId, id, cancellationToken);
                if (!result.Success)
                    return result.Error == "Forbidden"
                        ? Results.Forbid()
                        : Results.NotFound();

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapCreateMyVehicleEvent(this RouteGroupBuilder app)
        {
            return app.MapPost("/my-events", async (
                CreateMyVehicleEventDTO request,
                ClaimsPrincipal user,
                IVehicleEventsService vehicleEvents,
                CancellationToken cancellationToken) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var result = await vehicleEvents.CreateForUserAsync(userId, request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-events/{result.Data!.Id}",
                    result.Data
                );
            });
        }
    }
}
