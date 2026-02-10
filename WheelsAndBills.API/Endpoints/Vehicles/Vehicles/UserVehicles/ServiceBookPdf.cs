using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
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
                IWebHostEnvironment env,
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

                var mileages = await db.VehicleMileage
                    .AsNoTracking()
                    .Where(m => m.VehicleId == id)
                    .OrderByDescending(m => m.Date)
                    .ThenByDescending(m => m.Mileage)
                    .Select(m => new VehicleMileageRow(m.Date, m.Mileage))
                    .ToListAsync(cancellationToken);

                var parts = await db.EventParts
                    .AsNoTracking()
                    .Where(p => p.RepairEvent.VehicleEvent.VehicleId == id)
                    .OrderByDescending(p => p.RepairEvent.VehicleEvent.EventDate)
                    .Select(p => new VehiclePartRow(
                        p.RepairEvent.VehicleEvent.EventDate,
                        p.Part.Name,
                        p.Price
                    ))
                    .ToListAsync(cancellationToken);

                var webRoot = env.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(env.ContentRootPath, "wwwroot");
                }

                var logoPath = Path.Combine(webRoot, "logo", "logo.png");
                var bytes = GenerateServiceBookPdf(vehicle, events, mileages, parts, logoPath);
                var fileName = $"service_book_{vehicle.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

                return Results.File(bytes, "application/pdf", fileName);
            });
        }

        private static byte[] GenerateServiceBookPdf(
            dynamic vehicle,
            IReadOnlyList<ServiceBookRow> rows,
            IReadOnlyList<VehicleMileageRow> mileages,
            IReadOnlyList<VehiclePartRow> parts,
            string logoPath)
        {
            var generatedAt = DateTime.UtcNow;
            var latestMileage = mileages.FirstOrDefault()?.Mileage;
            var firstEvent = rows.LastOrDefault();
            var lastEvent = rows.FirstOrDefault();
            var partsTotal = parts.Sum(p => p.Price);
            var hasLogo = File.Exists(logoPath);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(28);
                    page.DefaultTextStyle(t => t.FontSize(11));

                    page.Header().Row(row =>
                    {
                        if (hasLogo)
                        {
                            row.ConstantItem(220)
                                .AlignMiddle()
                                .Height(140)
                                .Image(logoPath, ImageScaling.FitArea);
                        }

                        row.RelativeItem().AlignMiddle().AlignCenter().Column(col =>
                        {
                            col.Item().Text("Książka serwisowa")
                                .FontSize(18).SemiBold();
                            col.Item().Text($"Wygenerowano: {generatedAt:yyyy-MM-dd HH:mm} UTC")
                                .FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingVertical(6).LineHorizontal(1);

                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Pojazd: {vehicle.Brand} {vehicle.Model} ({vehicle.Year})")
                                    .SemiBold();
                                left.Item().Text($"VIN: {vehicle.Vin}");
                                left.Item().Text($"Typ: {vehicle.Type}");
                                left.Item().Text($"Status: {vehicle.Status}");
                            });

                            r.ConstantItem(200).Column(right =>
                            {
                                right.Item().Text("Podsumowanie").SemiBold();
                                right.Item().Text($"Liczba zdarzeń: {rows.Count}");
                                right.Item().Text($"Aktualny przebieg: {(latestMileage.HasValue ? $"{latestMileage.Value} km" : "brak danych")}");
                                right.Item().Text($"Pierwsze zdarzenie: {(firstEvent is null ? "-" : firstEvent.EventDate.ToString("yyyy-MM-dd"))}");
                                right.Item().Text($"Ostatnie zdarzenie: {(lastEvent is null ? "-" : lastEvent.EventDate.ToString("yyyy-MM-dd"))}");
                                right.Item().Text($"Wymienione części: {parts.Count} ({partsTotal:0.00} zł)");
                            });
                        });

                        col.Item().PaddingVertical(8).LineHorizontal(1);

                        if (mileages.Count > 0)
                        {
                            col.Item().Text("Historia przebiegu").FontSize(13).SemiBold();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(2);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Element(CellHeader).Text("Data");
                                    h.Cell().Element(CellHeader).Text("Przebieg (km)");
                                });

                                foreach (var m in mileages)
                                {
                                    table.Cell().Element(CellBody).Text(m.Date.ToString("yyyy-MM-dd"));
                                    table.Cell().Element(CellBody).Text(m.Mileage.ToString());
                                }
                            });
                            col.Item().PaddingVertical(8);
                        }

                        col.Item().Text("Historia zdarzeń").FontSize(13).SemiBold();
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
                                h.Cell().Element(CellHeader).Text("Data");
                                h.Cell().Element(CellHeader).Text("Typ");
                                h.Cell().Element(CellHeader).Text("Opis");
                                h.Cell().Element(CellHeader).Text("Przebieg");
                            });

                            if (rows.Count == 0)
                            {
                                var emptyCell = table.Cell();
                                emptyCell.ColumnSpan(4);
                                emptyCell.Element(CellBody).Text("Brak zdarzeń.");
                            }
                            else
                            {
                                foreach (var r in rows)
                                {
                                    table.Cell().Element(CellBody).Text(r.EventDate.ToString("yyyy-MM-dd"));
                                    table.Cell().Element(CellBody).Text(r.EventTypeName);
                                    table.Cell().Element(CellBody).Text(r.Description ?? "-");
                                    table.Cell().Element(CellBody).Text(r.Mileage.ToString());
                                }
                            }
                        });

                        col.Item().PaddingVertical(8);

                        col.Item().Text("Wymienione części").FontSize(13).SemiBold();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(4);
                                c.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Element(CellHeader).Text("Data");
                                h.Cell().Element(CellHeader).Text("Część");
                                h.Cell().Element(CellHeader).Text("Koszt (zł)");
                            });

                            if (parts.Count == 0)
                            {
                                var emptyCell = table.Cell();
                                emptyCell.ColumnSpan(3);
                                emptyCell.Element(CellBody).Text("Brak wymienionych części.");
                            }
                            else
                            {
                                foreach (var p in parts)
                                {
                                    table.Cell().Element(CellBody).Text(p.EventDate.ToString("yyyy-MM-dd"));
                                    table.Cell().Element(CellBody).Text(p.PartName);
                                    table.Cell().Element(CellBody).Text(p.Price.ToString("0.00"));
                                }
                            }
                        });
                    });

                    page.Footer()
                        .AlignRight()
                        .DefaultTextStyle(t => t.FontSize(9).FontColor(Colors.Grey.Medium))
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });
            }).GeneratePdf();
        }

        private sealed record ServiceBookRow(
            DateTime EventDate,
            int Mileage,
            string? Description,
            string EventTypeName);

        private sealed record VehicleMileageRow(
            DateTime Date,
            int Mileage);

        private sealed record VehiclePartRow(
            DateTime EventDate,
            string PartName,
            decimal Price);

        private static IContainer CellHeader(IContainer container)
        {
            return container
                .DefaultTextStyle(x => x.SemiBold())
                .PaddingVertical(4)
                .PaddingHorizontal(6)
                .Background(Colors.Grey.Lighten3)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten1);
        }

        private static IContainer CellBody(IContainer container)
        {
            return container
                .PaddingVertical(3)
                .PaddingHorizontal(6)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten3);
        }
    }
}
