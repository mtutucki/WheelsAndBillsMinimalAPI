using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.DTOs.Analytics;
using WheelsAndBills.Application.Features.Reports.ReportQueries;

namespace WheelsAndBills.API.Endpoints.Analytics
{
    public static class AnalyticsEndpoints
    {
        private static readonly Guid DeletedStatusId = Guid.Parse("85C30BAB-7FA3-4124-BE5D-1E220CACE01F");
        public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
        {
            var analytics = app
                .MapGroup("/analytics")
                .WithTags("Analytics")
                .RequireAuthorization();

            analytics.MapGetMileageSeries();
            analytics.MapGetMileageList();
            analytics.MapGetEventStatsSeries();
            analytics.MapGetEventList();
            analytics.MapGetFuelStatsSeries();
            analytics.MapGetFuelList();
            analytics.MapExportMileage();
            analytics.MapExportEvents();
            analytics.MapExportFuel();
            analytics.MapExportCosts();

            return app;
        }

        private static RouteHandlerBuilder MapGetMileageSeries(this RouteGroupBuilder group)
        {
            return group.MapGet("/mileage", [Authorize] async (
                Guid vehicleId,
                DateTime? from,
                DateTime? to,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == vehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                var query = db.VehicleMileage
                    .AsNoTracking()
                    .Where(m => m.VehicleId == vehicleId);

                if (from.HasValue)
                    query = query.Where(m => m.Date >= from.Value);

                if (to.HasValue)
                    query = query.Where(m => m.Date <= to.Value);

                var points = await query
                    .OrderBy(m => m.Date)
                    .Select(m => new MileagePointDto(m.Date, m.Mileage))
                    .ToListAsync(cancellationToken);

                return Results.Ok(points);
            });
        }

        private static RouteHandlerBuilder MapGetEventStatsSeries(this RouteGroupBuilder group)
        {
            return group.MapGet("/events", [Authorize] async (
                Guid vehicleId,
                Guid? eventTypeId,
                DateTime? from,
                DateTime? to,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == vehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                var query = db.VehicleEvents
                    .AsNoTracking()
                    .Where(e => e.VehicleId == vehicleId);

                if (eventTypeId.HasValue && eventTypeId.Value != Guid.Empty)
                    query = query.Where(e => e.EventTypeId == eventTypeId.Value);

                if (from.HasValue)
                    query = query.Where(e => e.EventDate >= from.Value);

                if (to.HasValue)
                    query = query.Where(e => e.EventDate <= to.Value);

                var perEvent = from e in query
                    join c in db.Costs.AsNoTracking() on e.Id equals c.VehicleEventId into costGroup
                    select new
                    {
                        Date = e.EventDate.Date,
                        Cost = costGroup.Sum(x => (decimal?)x.Amount) ?? 0m
                    };

                var perEventList = await perEvent.ToListAsync(cancellationToken);

                var points = perEventList
                    .GroupBy(x => x.Date)
                    .Select(g => new EventStatsPointDto(
                        g.Key,
                        g.Count(),
                        g.Sum(x => x.Cost)
                    ))
                    .OrderBy(p => p.Date)
                    .ToList();

                return Results.Ok(points);
            });
        }

        private static RouteHandlerBuilder MapGetEventList(this RouteGroupBuilder group)
        {
            return group.MapGet("/events/list", [Authorize] async (
                Guid vehicleId,
                Guid? eventTypeId,
                DateTime? from,
                DateTime? to,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == vehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                var query = db.VehicleEvents
                    .AsNoTracking()
                    .Where(e => e.VehicleId == vehicleId);

                if (eventTypeId.HasValue && eventTypeId.Value != Guid.Empty)
                    query = query.Where(e => e.EventTypeId == eventTypeId.Value);

                if (from.HasValue)
                    query = query.Where(e => e.EventDate >= from.Value);

                if (to.HasValue)
                    query = query.Where(e => e.EventDate <= to.Value);

                var items = await query
                    .OrderByDescending(e => e.EventDate)
                    .Select(e => new EventListItemDto(
                        e.Id,
                        e.EventDate,
                        e.EventType.Name,
                        e.Mileage,
                        db.Costs
                            .Where(c => c.VehicleEventId == e.Id)
                            .Select(c => (decimal?)c.Amount)
                            .Sum() ?? 0m,
                        e.Description
                    ))
                    .ToListAsync(cancellationToken);

                return Results.Ok(items);
            });
        }

        private static RouteHandlerBuilder MapGetFuelStatsSeries(this RouteGroupBuilder group)
        {
            return group.MapGet("/fuel", [Authorize] async (
                Guid vehicleId,
                DateTime? from,
                DateTime? to,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == vehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                var query = from f in db.FuelingEvents.AsNoTracking()
                            join e in db.VehicleEvents.AsNoTracking() on f.VehicleEventId equals e.Id
                            where e.VehicleId == vehicleId
                            select new
                            {
                                e.EventDate,
                                e.Mileage,
                                f.Liters,
                                f.TotalPrice
                            };

                if (from.HasValue)
                    query = query.Where(x => x.EventDate >= from.Value);

                if (to.HasValue)
                    query = query.Where(x => x.EventDate <= to.Value);

                var rows = await query
                    .OrderBy(x => x.EventDate)
                    .ThenBy(x => x.Mileage)
                    .Select(x => new FuelRow(
                        x.EventDate.Date,
                        x.Mileage,
                        x.Liters,
                        x.TotalPrice
                    ))
                    .ToListAsync(cancellationToken);

                var points = BuildFuelPoints(rows);

                return Results.Ok(points);
            });
        }

        private static RouteHandlerBuilder MapGetFuelList(this RouteGroupBuilder group)
        {
            return group.MapGet("/fuel/list", [Authorize] async (
                Guid vehicleId,
                DateTime? from,
                DateTime? to,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == vehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                var query = from f in db.FuelingEvents.AsNoTracking()
                            join e in db.VehicleEvents.AsNoTracking() on f.VehicleEventId equals e.Id
                            where e.VehicleId == vehicleId
                            select new
                            {
                                e.EventDate,
                                e.Mileage,
                                f.Liters,
                                f.TotalPrice
                            };

                if (from.HasValue)
                    query = query.Where(x => x.EventDate >= from.Value);

                if (to.HasValue)
                    query = query.Where(x => x.EventDate <= to.Value);

                var items = await query
                    .OrderByDescending(x => x.EventDate)
                    .ThenByDescending(x => x.Mileage)
                    .Select(x => new FuelListItemDto(
                        x.EventDate.Date,
                        x.Mileage,
                        x.Liters,
                        x.TotalPrice,
                        x.Liters > 0 ? x.TotalPrice / x.Liters : 0m
                    ))
                    .ToListAsync(cancellationToken);

                return Results.Ok(items);
            });
        }

        private static RouteHandlerBuilder MapGetMileageList(this RouteGroupBuilder group)
        {
            return group.MapGet("/mileage/list", [Authorize] async (
                Guid vehicleId,
                DateTime? from,
                DateTime? to,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == vehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                var query = db.VehicleMileage
                    .AsNoTracking()
                    .Where(m => m.VehicleId == vehicleId);

                if (from.HasValue)
                    query = query.Where(m => m.Date >= from.Value);

                if (to.HasValue)
                    query = query.Where(m => m.Date <= to.Value);

                var items = await query
                    .OrderByDescending(m => m.Date)
                    .Select(m => new MileageListItemDto(m.Date, m.Mileage))
                    .ToListAsync(cancellationToken);

                return Results.Ok(items);
            });
        }

        private static RouteHandlerBuilder MapExportMileage(this RouteGroupBuilder group)
        {
            return group.MapPost("/mileage/export", [Authorize] async (
                MileageExportRequest request,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == request.VehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                var query = db.VehicleMileage
                    .AsNoTracking()
                    .Where(m => m.VehicleId == request.VehicleId);

                if (request.From.HasValue)
                    query = query.Where(m => m.Date >= request.From.Value);

                if (request.To.HasValue)
                    query = query.Where(m => m.Date <= request.To.Value);

                var points = await query
                    .OrderBy(m => m.Date)
                    .Select(m => new MileagePointDto(m.Date, m.Mileage))
                    .ToListAsync(cancellationToken);

                using var workbook = new XLWorkbook();
                var sheet = workbook.Worksheets.Add("Przebieg");
                sheet.Cell(1, 1).Value = "Data";
                sheet.Cell(1, 2).Value = "Przebieg (km)";

                for (var i = 0; i < points.Count; i++)
                {
                    sheet.Cell(i + 2, 1).Value = points[i].Date.ToString("yyyy-MM-dd");
                    sheet.Cell(i + 2, 2).Value = points[i].Mileage;
                }

                sheet.Columns().AdjustToContents();

                TryAddChartImage(sheet, request.ChartImageBase64, points.Count + 3, 1);

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"analiza-przebiegu-{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
                return Results.File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            });
        }

        private static RouteHandlerBuilder MapExportEvents(this RouteGroupBuilder group)
        {
            return group.MapPost("/events/export", [Authorize] async (
                EventsExportRequest request,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == request.VehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                var baseQuery = db.VehicleEvents
                    .AsNoTracking()
                    .Where(e => e.VehicleId == request.VehicleId);

                if (request.EventTypeId.HasValue && request.EventTypeId.Value != Guid.Empty)
                    baseQuery = baseQuery.Where(e => e.EventTypeId == request.EventTypeId.Value);

                if (request.From.HasValue)
                    baseQuery = baseQuery.Where(e => e.EventDate >= request.From.Value);

                if (request.To.HasValue)
                    baseQuery = baseQuery.Where(e => e.EventDate <= request.To.Value);

                var perEvent = from e in baseQuery
                    join c in db.Costs.AsNoTracking() on e.Id equals c.VehicleEventId into costGroup
                    select new
                    {
                        Date = e.EventDate.Date,
                        Cost = costGroup.Sum(x => (decimal?)x.Amount) ?? 0m
                    };

                var perEventList = await perEvent.ToListAsync(cancellationToken);

                var points = perEventList
                    .GroupBy(x => x.Date)
                    .Select(g => new EventStatsPointDto(
                        g.Key,
                        g.Count(),
                        g.Sum(x => x.Cost)
                    ))
                    .OrderBy(p => p.Date)
                    .ToList();

                var items = await baseQuery
                    .OrderByDescending(e => e.EventDate)
                    .Select(e => new EventListItemDto(
                        e.Id,
                        e.EventDate,
                        e.EventType.Name,
                        e.Mileage,
                        db.Costs
                            .Where(c => c.VehicleEventId == e.Id)
                            .Select(c => (decimal?)c.Amount)
                            .Sum() ?? 0m,
                        e.Description
                    ))
                    .ToListAsync(cancellationToken);

                using var workbook = new XLWorkbook();

                var summary = workbook.Worksheets.Add("Podsumowanie");
                summary.Cell(1, 1).Value = "Data";
                summary.Cell(1, 2).Value = "Ilość zdarzeń";
                summary.Cell(1, 3).Value = "Koszty (zł)";

                for (var i = 0; i < points.Count; i++)
                {
                    summary.Cell(i + 2, 1).Value = points[i].Date.ToString("yyyy-MM-dd");
                    summary.Cell(i + 2, 2).Value = points[i].Count;
                    summary.Cell(i + 2, 3).Value = points[i].Cost;
                }

                summary.Columns().AdjustToContents();

                TryAddChartImage(summary, request.ChartImageBase64, points.Count + 3, 1);

                var list = workbook.Worksheets.Add("Zdarzenia");
                list.Cell(1, 1).Value = "Data";
                list.Cell(1, 2).Value = "Typ";
                list.Cell(1, 3).Value = "Przebieg (km)";
                list.Cell(1, 4).Value = "Koszt (zł)";
                list.Cell(1, 5).Value = "Opis";

                for (var i = 0; i < items.Count; i++)
                {
                    list.Cell(i + 2, 1).Value = items[i].EventDate.ToString("yyyy-MM-dd");
                    list.Cell(i + 2, 2).Value = items[i].EventType;
                    list.Cell(i + 2, 3).Value = items[i].Mileage;
                    list.Cell(i + 2, 4).Value = items[i].Cost;
                    list.Cell(i + 2, 5).Value = items[i].Description ?? string.Empty;
                }

                list.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"analiza-zdarzen-{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
                return Results.File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            });
        }

        private static RouteHandlerBuilder MapExportFuel(this RouteGroupBuilder group)
        {
            return group.MapPost("/fuel/export", [Authorize] async (
                FuelExportRequest request,
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == request.VehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                var query = from f in db.FuelingEvents.AsNoTracking()
                            join e in db.VehicleEvents.AsNoTracking() on f.VehicleEventId equals e.Id
                            where e.VehicleId == request.VehicleId
                            select new
                            {
                                e.EventDate,
                                e.Mileage,
                                f.Liters,
                                f.TotalPrice
                            };

                if (request.From.HasValue)
                    query = query.Where(x => x.EventDate >= request.From.Value);

                if (request.To.HasValue)
                    query = query.Where(x => x.EventDate <= request.To.Value);

                var rows = await query
                    .OrderBy(x => x.EventDate)
                    .ThenBy(x => x.Mileage)
                    .Select(x => new FuelRow(
                        x.EventDate.Date,
                        x.Mileage,
                        x.Liters,
                        x.TotalPrice
                    ))
                    .ToListAsync(cancellationToken);

                var points = BuildFuelPoints(rows);

                using var workbook = new XLWorkbook();
                var sheet = workbook.Worksheets.Add("Paliwo");
                sheet.Cell(1, 1).Value = "Data";
                sheet.Cell(1, 2).Value = "Przebieg (km)";
                sheet.Cell(1, 3).Value = "Dystans (km)";
                sheet.Cell(1, 4).Value = "Litry";
                sheet.Cell(1, 5).Value = "Koszt (zł)";
                sheet.Cell(1, 6).Value = "Cena/L (zł)";
                sheet.Cell(1, 7).Value = "Spalanie (l/100km)";
                sheet.Cell(1, 8).Value = "Koszt/100km (zł)";

                for (var i = 0; i < points.Count; i++)
                {
                    var row = points[i];
                    sheet.Cell(i + 2, 1).Value = row.Date.ToString("yyyy-MM-dd");
                    sheet.Cell(i + 2, 2).Value = row.Mileage;
                    sheet.Cell(i + 2, 3).Value = row.Distance;
                    sheet.Cell(i + 2, 4).Value = row.Liters;
                    sheet.Cell(i + 2, 5).Value = row.TotalCost;
                    sheet.Cell(i + 2, 6).Value = row.PricePerLiter;
                    sheet.Cell(i + 2, 7).Value = row.ConsumptionPer100;
                    sheet.Cell(i + 2, 8).Value = row.CostPer100;
                }

                sheet.Columns().AdjustToContents();

                TryAddChartImage(sheet, request.ChartImageBase64, points.Count + 3, 1);

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"analiza-paliwo-{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
                return Results.File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            });
        }

        private static RouteHandlerBuilder MapExportCosts(this RouteGroupBuilder group)
        {
            return group.MapPost("/costs/export", [Authorize] async (
                CostsExportRequest request,
                ClaimsPrincipal user,
                IAppDbContext db,
                IReportQueriesService reportQueries,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var ownsVehicle = await db.Vehicles
                    .AnyAsync(v => v.Id == request.VehicleId && v.UserId == userId && v.StatusId != DeletedStatusId, cancellationToken);

                if (!ownsVehicle)
                    return Results.NotFound();

                if (!request.From.HasValue || !request.To.HasValue)
                    return Results.BadRequest("Missing range");

                var rows = await reportQueries.GetMonthlyCostsAsync(
                    request.VehicleId,
                    request.From.Value,
                    request.To.Value,
                    cancellationToken);

                var filtered = rows
                    .Where(r =>
                        r.TotalAmount != 0m ||
                        r.FuelAmount != 0m ||
                        r.RepairLaborAmount != 0m ||
                        r.RepairPartsAmount != 0m ||
                        r.OtherAmount != 0m ||
                        r.EventsCount != 0)
                    .OrderBy(r => r.Year)
                    .ThenBy(r => r.Month)
                    .ToList();

                if (filtered.Count == 0)
                    return Results.BadRequest("No data");

                using var workbook = new XLWorkbook();

                var summary = workbook.Worksheets.Add("KosztyPodsumowanie");
                summary.Cell(1, 1).Value = "Miesiac";
                summary.Cell(1, 2).Value = "Koszt calkowity (zl)";
                summary.Cell(1, 3).Value = "Liczba zdarzen";

                for (var i = 0; i < filtered.Count; i++)
                {
                    var row = filtered[i];
                    summary.Cell(i + 2, 1).Value = $"{row.Year}-{row.Month:00}";
                    summary.Cell(i + 2, 2).Value = row.TotalAmount;
                    summary.Cell(i + 2, 3).Value = row.EventsCount;
                }

                summary.Columns().AdjustToContents();
                TryAddChartImage(summary, request.TrendChartImageBase64, filtered.Count + 3, 1);

                var categories = workbook.Worksheets.Add("KosztyKategorie");
                categories.Cell(1, 1).Value = "Miesiac";
                categories.Cell(1, 2).Value = "Paliwo (zl)";
                categories.Cell(1, 3).Value = "Robocizna (zl)";
                categories.Cell(1, 4).Value = "Czesci (zl)";
                categories.Cell(1, 5).Value = "Inne (zl)";

                for (var i = 0; i < filtered.Count; i++)
                {
                    var row = filtered[i];
                    categories.Cell(i + 2, 1).Value = $"{row.Year}-{row.Month:00}";
                    categories.Cell(i + 2, 2).Value = row.FuelAmount;
                    categories.Cell(i + 2, 3).Value = row.RepairLaborAmount;
                    categories.Cell(i + 2, 4).Value = row.RepairPartsAmount;
                    categories.Cell(i + 2, 5).Value = row.OtherAmount;
                }

                categories.Columns().AdjustToContents();
                TryAddChartImage(categories, request.CategoryChartImageBase64, filtered.Count + 3, 1);

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"analiza-kosztow-{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
                return Results.File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            });
        }

        private static void TryAddChartImage(IXLWorksheet sheet, string? dataUrl, int row, int column)
        {
            if (string.IsNullOrWhiteSpace(dataUrl))
                return;

            var base64 = dataUrl;
            var commaIndex = base64.IndexOf(',');
            if (commaIndex >= 0)
                base64 = base64[(commaIndex + 1)..];

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64);
            }
            catch
            {
                return;
            }

            using var stream = new MemoryStream(bytes);
            sheet.AddPicture(stream)
                .MoveTo(sheet.Cell(row, column))
                .WithSize(640, 320);
        }

        private static List<FuelStatsPointDto> BuildFuelPoints(
            IEnumerable<FuelRow> rows)
        {
            FuelRow? prev = null;
            var points = new List<FuelStatsPointDto>();

            foreach (var row in rows)
            {
                if (row.Mileage <= 0 || row.Liters <= 0)
                    continue;

                if (prev is not null)
                {
                    var distance = row.Mileage - prev.Mileage;
                    if (distance > 0)
                    {
                        var pricePerLiter = row.TotalCost / row.Liters;
                        var consumptionPer100 = (row.Liters / distance) * 100m;
                        var costPer100 = (row.TotalCost / distance) * 100m;

                        points.Add(new FuelStatsPointDto(
                            row.Date,
                            row.Mileage,
                            distance,
                            row.Liters,
                            row.TotalCost,
                            pricePerLiter,
                            consumptionPer100,
                            costPer100
                        ));
                    }
                }

                prev = row;
            }

            return points;
        }

        private sealed record MileageExportRequest(
            Guid VehicleId,
            DateTime? From,
            DateTime? To,
            string? ChartImageBase64);

        private sealed record EventsExportRequest(
            Guid VehicleId,
            Guid? EventTypeId,
            DateTime? From,
            DateTime? To,
            string? ChartImageBase64);

        private sealed record FuelExportRequest(
            Guid VehicleId,
            DateTime? From,
            DateTime? To,
            string? ChartImageBase64);

        private sealed record CostsExportRequest(
            Guid VehicleId,
            DateTime? From,
            DateTime? To,
            string? TrendChartImageBase64,
            string? CategoryChartImageBase64);

        private sealed record FuelRow(
            DateTime Date,
            int Mileage,
            decimal Liters,
            decimal TotalCost);
    }
}
