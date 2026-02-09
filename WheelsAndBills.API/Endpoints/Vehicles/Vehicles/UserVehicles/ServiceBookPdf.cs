using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WheelsAndBills.Application.Abstractions.Persistence;

namespace WheelsAndBills.API.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class ServiceBookPdf
    {
        public static RouteHandlerBuilder MapGetServiceBookPdf(this RouteGroupBuilder group)
        {
            return group.MapGet("/{id:guid}/service-book", [Authorize] async (
                Guid id,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var vehicle = await db.Vehicles
                    .AsNoTracking()
                    .Where(v => v.Id == id && v.UserId == userId)
                    .Select(v => new
                    {
                        v.Id,
                        v.Vin,
                        v.Year,
                        Brand = v.Brand.Name,
                        Model = v.Model.Name,
                        Type = v.Type.Name,
                        Status = v.Status.Name
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (vehicle is null)
                    return Results.NotFound();

                var events = await db.VehicleEvents
                    .AsNoTracking()
                    .Where(e => e.VehicleId == id)
                    .OrderByDescending(e => e.EventDate)
                    .Select(e => new ServiceBookRow(
                        e.EventDate,
                        e.Mileage,
                        e.Description,
                        e.EventType.Name
                    ))
                    .ToListAsync(cancellationToken);

                var bytes = GenerateServiceBookPdf(vehicle, events);
                var fileName = $"service_book_{vehicle.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

                return Results.File(bytes, "application/pdf", fileName);
            });
        }

        private static byte[] GenerateServiceBookPdf(
            dynamic vehicle,
            IReadOnlyList<ServiceBookRow> rows)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Książka serwisowa").FontSize(16).SemiBold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Pojazd: {vehicle.Brand} {vehicle.Model} ({vehicle.Year})");
                        col.Item().Text($"VIN: {vehicle.Vin}");
                        col.Item().Text($"Typ: {vehicle.Type} | Status: {vehicle.Status}");
                        col.Item().PaddingVertical(10).LineHorizontal(1);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(4);
                                c.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Text("Data").SemiBold();
                                h.Cell().Text("Typ").SemiBold();
                                h.Cell().Text("Opis").SemiBold();
                                h.Cell().Text("Przebieg").SemiBold();
                            });

                            foreach (var r in rows)
                            {
                                table.Cell().Text(r.EventDate.ToString("yyyy-MM-dd"));
                                table.Cell().Text(r.EventTypeName);
                                table.Cell().Text(r.Description ?? "-");
                                table.Cell().Text(r.Mileage.ToString());
                            }
                        });
                    });
                });
            }).GeneratePdf();
        }

        private sealed record ServiceBookRow(
            DateTime EventDate,
            int Mileage,
            string? Description,
            string EventTypeName);
    }
}
