using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using WheelsAndBills.Application.Features.Events.FuelingEvents;
using WheelsAndBills.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public static RouteHandlerBuilder MapGetFuelingSummaryForUser(this RouteGroupBuilder app)
        {
            return app.MapGet("/summary", async (
                ClaimsPrincipal user,
                IAppDbContext db,
                Guid? vehicleId,
                DateTime? from,
                DateTime? to,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var query = db.FuelingEvents
                    .AsNoTracking()
                    .Where(f => f.VehicleEvent.Vehicle.UserId == userId);

                if (vehicleId.HasValue)
                    query = query.Where(f => f.VehicleEvent.VehicleId == vehicleId.Value);

                if (from.HasValue)
                    query = query.Where(f => f.VehicleEvent.EventDate >= from.Value.Date);

                if (to.HasValue)
                    query = query.Where(f => f.VehicleEvent.EventDate <= to.Value.Date);

                var items = await query
                    .GroupBy(f => new
                    {
                        Year = f.VehicleEvent.EventDate.Year,
                        Month = f.VehicleEvent.EventDate.Month
                    })
                    .Select(g => new FuelingSummaryRow(
                        g.Key.Year,
                        g.Key.Month,
                        g.Sum(x => x.Liters),
                        g.Sum(x => x.TotalPrice),
                        g.Sum(x => x.Liters) == 0
                            ? 0
                            : g.Sum(x => x.TotalPrice) / g.Sum(x => x.Liters)
                    ))
                    .OrderBy(r => r.Year)
                    .ThenBy(r => r.Month)
                    .ToListAsync(cancellationToken);

                return Results.Ok(items);
            });
        }
    }
}
