using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using System.Security.Claims;
using WheelsAndBills.Application.Features.Events.VehicleEvents;
using WheelsAndBills.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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

        public static RouteHandlerBuilder MapExportMyTimelinePdf(this RouteGroupBuilder app)
        {
            return app.MapGet("/timeline-export", async (
                ClaimsPrincipal user,
                IAppDbContext db,
                Guid? vehicleId,
                Guid? eventTypeId,
                DateTime? from,
                DateTime? to,
                CancellationToken cancellationToken) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var query = db.VehicleEvents
                    .AsNoTracking()
                    .Where(e => e.Vehicle.UserId == userId);

                if (vehicleId.HasValue)
                    query = query.Where(e => e.VehicleId == vehicleId.Value);

                if (eventTypeId.HasValue)
                    query = query.Where(e => e.EventTypeId == eventTypeId.Value);

                if (from.HasValue)
                    query = query.Where(e => e.EventDate >= from.Value.Date);

                if (to.HasValue)
                    query = query.Where(e => e.EventDate <= to.Value.Date);

                var items = await query
                    .OrderByDescending(e => e.EventDate)
                    .Select(e => new TimelineRow(
                        e.EventDate,
                        e.Mileage,
                        e.Description,
                        e.EventType.Name,
                        e.Vehicle.Brand.Name,
                        e.Vehicle.Model.Name,
                        e.Vehicle.Year
                    ))
                    .ToListAsync(cancellationToken);

                var fileName = $"timeline_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                var bytes = GenerateTimelinePdf(items, from, to);

                return Results.File(bytes, "application/pdf", fileName);
            });
        }

        private static byte[] GenerateTimelinePdf(
            IReadOnlyList<TimelineRow> items,
            DateTime? from,
            DateTime? to)
        {
            var rangeText = from.HasValue || to.HasValue
                ? $" ({from:yyyy-MM-dd} - {to:yyyy-MM-dd})"
                : string.Empty;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Historia zdarzeń{rangeText}").FontSize(16).SemiBold();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(1);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                            c.RelativeColumn(3);
                            c.RelativeColumn(2);
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Data").SemiBold();
                            h.Cell().Text("Typ").SemiBold();
                            h.Cell().Text("Pojazd").SemiBold();
                            h.Cell().Text("Opis").SemiBold();
                            h.Cell().Text("Przebieg").SemiBold();
                        });

                        foreach (var r in items)
                        {
                            table.Cell().Text(r.EventDate.ToString("yyyy-MM-dd"));
                            table.Cell().Text(r.EventTypeName);
                            table.Cell().Text($"{r.Brand} {r.Model} ({r.Year})");
                            table.Cell().Text(r.Description ?? "-");
                            table.Cell().Text(r.Mileage.ToString());
                        }
                    });
                });
            }).GeneratePdf();
        }

        private sealed record TimelineRow(
            DateTime EventDate,
            int Mileage,
            string? Description,
            string EventTypeName,
            string Brand,
            string Model,
            int Year);
    }
}
