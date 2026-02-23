using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;
using WheelsAndBills.Application.Features.Reports.Reports;
using WheelsAndBills.Application.DTOs.Reports;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Features.Reports.ReportQueries;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IO;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WheelsAndBills.API.Endpoints.Reports.Reports
{
    public static class ReportsEndpoints
    {
        private sealed record VehicleReportInfo(
            Guid VehicleId,
            string Brand,
            string Model,
            int Year,
            string Vin,
            string Type,
            string Status,
            string OwnerName,
            string? OwnerEmail,
            string? OwnerCity,
            string? OwnerCountry
        );

        public static RouteHandlerBuilder MapGetMyReports(this RouteGroupBuilder app)
        {
            return app.MapGet("/my", async (
                ClaimsPrincipal user,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var items = await db.Reports
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ReportListItemDto(
                        r.Id,
                        r.Definition.Code,
                        r.CreatedAt,
                        r.Parameters
                            .OrderBy(p => p.Name)
                            .Select(p => new ReportParameterItemDto(p.Name, p.Value))
                            .ToList(),
                        db.GeneratedReports
                            .Where(g => g.ReportId == r.Id)
                            .OrderByDescending(g => g.Id)
                            .Select(g => new GeneratedReportItemDto(g.Id, g.FilePath))
                            .ToList()
                    ))
                    .ToListAsync(cancellationToken);

                return Results.Ok(items);
            })
            .RequireAuthorization();
        }

        public static RouteHandlerBuilder MapDeleteMyReport(this RouteGroupBuilder app)
        {
            return app.MapDelete("/my/{id:guid}", async (
                ClaimsPrincipal user,
                Guid id,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var report = await db.Reports
                    .Where(r => r.Id == id && r.UserId == userId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (report is null)
                    return Results.NotFound();

                var generated = await db.GeneratedReports
                    .Where(g => g.ReportId == id)
                    .ToListAsync(cancellationToken);

                var filesToDelete = generated
                    .Select(g => g.FilePath)
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .ToList();

                var parameters = await db.ReportParameters
                    .Where(p => p.ReportId == id)
                    .ToListAsync(cancellationToken);

                db.GeneratedReports.RemoveRange(generated);
                db.ReportParameters.RemoveRange(parameters);
                db.Reports.Remove(report);

                await db.SaveChangesAsync(cancellationToken);

                foreach (var path in filesToDelete)
                {
                    try
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                    catch
                    {
                    }
                }

                return Results.NoContent();
            })
            .RequireAuthorization();
        }

        public static RouteHandlerBuilder MapGetMyReportById(this RouteGroupBuilder app)
        {
            return app.MapGet("/my/{id:guid}", async (
                ClaimsPrincipal user,
                Guid id,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var report = await db.Reports
                    .Where(r => r.Id == id && r.UserId == userId)
                    .Select(r => new
                    {
                        r.Id,
                        r.CreatedAt,
                        DefinitionCode = r.Definition.Code
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (report is null)
                    return Results.NotFound();

                var parameters = await db.ReportParameters
                    .Where(p => p.ReportId == id)
                    .OrderBy(p => p.Name)
                    .Select(p => new ReportParameterItemDto(p.Name, p.Value))
                    .ToListAsync(cancellationToken);

                var generated = await db.GeneratedReports
                    .Where(g => g.ReportId == id)
                    .OrderByDescending(g => g.Id)
                    .Select(g => new GeneratedReportItemDto(g.Id, g.FilePath))
                    .ToListAsync(cancellationToken);

                var dto = new ReportDetailDto(
                    report.Id,
                    report.DefinitionCode,
                    report.CreatedAt,
                    parameters,
                    generated
                );

                return Results.Ok(dto);
            })
            .RequireAuthorization();
        }

        public static RouteHandlerBuilder MapDownloadReport(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}/download", async (
                ClaimsPrincipal user,
                Guid id,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var reportExists = await db.Reports
                    .AnyAsync(r => r.Id == id && r.UserId == userId, cancellationToken);
                if (!reportExists)
                    return Results.NotFound();

                var filePath = await db.GeneratedReports
                    .Where(g => g.ReportId == id)
                    .OrderByDescending(g => g.Id)
                    .Select(g => g.FilePath)
                    .FirstOrDefaultAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(filePath))
                    return Results.NotFound();

                if (!File.Exists(filePath))
                    return Results.NotFound();

                var fileName = Path.GetFileName(filePath);
                return Results.File(filePath, "application/octet-stream", fileName);
            })
            .RequireAuthorization();
        }

        public static RouteHandlerBuilder MapDownloadGeneratedReport(this RouteGroupBuilder app)
        {
            return app.MapGet("/generated/{generatedId:guid}/download", async (
                ClaimsPrincipal user,
                Guid generatedId,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var item = await db.GeneratedReports
                    .Where(g => g.Id == generatedId)
                    .Select(g => new
                    {
                        g.FilePath,
                        ReportUserId = g.Report.UserId
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (item is null || item.ReportUserId != userId)
                    return Results.NotFound();

                if (string.IsNullOrWhiteSpace(item.FilePath) || !File.Exists(item.FilePath))
                    return Results.NotFound();

                var fileName = Path.GetFileName(item.FilePath);
                return Results.File(item.FilePath, "application/octet-stream", fileName);
            })
            .RequireAuthorization();
        }

        public static RouteHandlerBuilder MapGenerateReportFiles(this RouteGroupBuilder app)
        {
            return app.MapPost("/{id:guid}/generate", async (
                ClaimsPrincipal user,
                Guid id,
                IAppDbContext db,
                IReportQueriesService reportQueries,
                IHostEnvironment env,
                ILoggerFactory loggerFactory,
                CancellationToken cancellationToken) =>
            {
                var logger = loggerFactory.CreateLogger("ReportsEndpoints");
                var existingConnectionString = db.Database.GetDbConnection().ConnectionString;
                if (string.IsNullOrWhiteSpace(existingConnectionString))
                {
                    db.Database.SetConnectionString(
                        "Server=localhost;Database=WheelsAndBillsAPI;Trusted_Connection=True;TrustServerCertificate=True"
                    );
                    existingConnectionString = db.Database.GetDbConnection().ConnectionString;
                }
                if (string.IsNullOrWhiteSpace(existingConnectionString))
                {
                    logger.LogError("ConnectionString is empty before report generation.");
                    return Results.Problem("Database connection string is not configured.");
                }

                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var report = await db.Reports
                    .Where(r => r.Id == id && r.UserId == userId)
                    .Select(r => new
                    {
                        r.Id,
                        DefinitionCode = r.Definition.Code
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (report is null)
                    return Results.NotFound();

                var parameters = await db.ReportParameters
                    .Where(p => p.ReportId == id)
                    .ToDictionaryAsync(p => p.Name, p => p.Value, cancellationToken);

                if (!TryParseParams(parameters, out var vehicleId, out var from, out var to, out var error))
                    return Results.BadRequest(error);

                var vehicleInfo = await db.Vehicles
                    .Where(v => v.Id == vehicleId)
                    .Select(v => new VehicleReportInfo(
                        v.Id,
                        v.Brand.Name,
                        v.Model.Name,
                        v.Year,
                        v.Vin,
                        v.Type.Name,
                        v.Status.Name,
                        (v.User.FirstName + " " + v.User.LastName).Trim(),
                        v.User.Email,
                        v.User.City,
                        v.User.Country
                    ))
                    .FirstOrDefaultAsync(cancellationToken);

                if (vehicleInfo is null)
                    return Results.NotFound("Vehicle not found.");

                var reportsRoot = Path.Combine(env.ContentRootPath, "ReportsFiles");
                Directory.CreateDirectory(reportsRoot);

                var baseName = report.DefinitionCode switch
                {
                    "MONTHLY_COSTS" => $"KOSZTY_MIESIECZNE_{report.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
                    "COSTS_BY_EVENT_TYPE" => $"KOSZTY_WG_TYPU_{report.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
                    "REPAIRS_HISTORY" => $"HISTORIA_NAPRAW_{report.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
                    _ => $"{report.DefinitionCode}_{report.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}"
                };
                var pdfPath = Path.Combine(reportsRoot, $"{baseName}.pdf");
                var xlsxPath = Path.Combine(reportsRoot, $"{baseName}.xlsx");

                if (report.DefinitionCode == "MONTHLY_COSTS")
                {
                    var rows = await reportQueries.GetMonthlyCostsAsync(vehicleId, from, to, cancellationToken);
                    GenerateMonthlyCostsPdf(rows, vehicleInfo, from, to, pdfPath);
                    GenerateMonthlyCostsExcel(rows, vehicleInfo, from, to, xlsxPath);
                }
                else if (report.DefinitionCode == "COSTS_BY_EVENT_TYPE")
                {
                    var rows = await reportQueries.GetCostsByEventTypeAsync(vehicleId, from, to, cancellationToken);
                    GenerateCostsByTypePdf(rows, vehicleInfo, from, to, pdfPath);
                    GenerateCostsByTypeExcel(rows, vehicleInfo, from, to, xlsxPath);
                }
                else if (report.DefinitionCode == "REPAIRS_HISTORY")
                {
                    var rows = await reportQueries.GetRepairHistoryAsync(vehicleId, from, to, cancellationToken);
                    GenerateRepairsHistoryPdf(rows, vehicleInfo, from, to, pdfPath);
                    GenerateRepairsHistoryExcel(rows, vehicleInfo, from, to, xlsxPath);
                }
                else
                {
                    return Results.BadRequest("Unsupported report definition");
                }

                var pdfGeneratedId = Guid.NewGuid();

                db.GeneratedReports.Add(new WheelsAndBills.Domain.Entities.Report.GeneratedReport
                {
                    Id = pdfGeneratedId,
                    ReportId = report.Id,
                    FilePath = pdfPath
                });

                var excelGeneratedId = Guid.NewGuid();
                db.GeneratedReports.Add(new WheelsAndBills.Domain.Entities.Report.GeneratedReport
                {
                    Id = excelGeneratedId,
                    ReportId = report.Id,
                    FilePath = xlsxPath
                });

                await TryCreateReportReadyNotificationAsync(
                    db,
                    userId,
                    vehicleId,
                    report.DefinitionCode,
                    cancellationToken);

                await db.SaveChangesAsync(cancellationToken);

                return Results.Ok(new { PdfId = pdfGeneratedId, ExcelId = excelGeneratedId });
            })
            .RequireAuthorization();
        }

        private static bool TryParseParams(
            IReadOnlyDictionary<string, string> parameters,
            out Guid vehicleId,
            out DateTime from,
            out DateTime to,
            out string? error)
        {
            error = null;
            vehicleId = Guid.Empty;
            from = default;
            to = default;

            if (!parameters.TryGetValue("vehicleId", out var vehicleStr) || !Guid.TryParse(vehicleStr, out vehicleId))
            {
                error = "Missing or invalid vehicleId parameter";
                return false;
            }

            if (!parameters.TryGetValue("from", out var fromStr) || !DateTime.TryParse(fromStr, out from))
            {
                error = "Missing or invalid from parameter";
                return false;
            }

            if (!parameters.TryGetValue("to", out var toStr) || !DateTime.TryParse(toStr, out to))
            {
                error = "Missing or invalid to parameter";
                return false;
            }

            return true;
        }

        private static async Task TryCreateReportReadyNotificationAsync(
            IAppDbContext db,
            Guid userId,
            Guid vehicleId,
            string reportCode,
            CancellationToken ct)
        {
            var typeId = await EnsureNotificationTypeAsync(db, "REPORT_READY", ct);
            if (!await IsNotificationEnabledAsync(db, userId, typeId, ct))
                return;

            var reportLabel = reportCode switch
            {
                "MONTHLY_COSTS" => "koszty miesięczne",
                "COSTS_BY_EVENT_TYPE" => "koszty wg typu",
                "REPAIRS_HISTORY" => "historia napraw",
                _ => reportCode
            };
            var message = $"Twój raport \"{reportLabel}\" jest gotowy do pobrania.";

            db.Notifications.Add(new WheelsAndBills.Domain.Entities.Notification.Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                VehicleId = vehicleId,
                NotificationTypeId = typeId,
                Title = "Raport gotowy",
                Message = message,
                ScheduledAt = DateTime.UtcNow,
                IsSent = false,
                IsRead = false
            });
        }

        private static async Task<Guid?> EnsureNotificationTypeAsync(
            IAppDbContext db,
            string code,
            CancellationToken ct)
        {
            var existing = await db.NotificationTypes
                .Where(t => t.Code == code)
                .Select(t => new { t.Id })
                .FirstOrDefaultAsync(ct);

            if (existing is not null)
                return existing.Id;

            var type = new WheelsAndBills.Domain.Entities.Notification.NotificationType
            {
                Id = Guid.NewGuid(),
                Code = code
            };

            db.NotificationTypes.Add(type);
            await db.SaveChangesAsync(ct);

            return type.Id;
        }

        private static async Task<bool> IsNotificationEnabledAsync(
            IAppDbContext db,
            Guid userId,
            Guid? notificationTypeId,
            CancellationToken ct)
        {
            if (notificationTypeId is null)
                return false;

            var pref = await db.NotificationPreferences
                .Where(p => p.UserId == userId && p.NotificationTypeId == notificationTypeId)
                .Select(p => (bool?)p.IsEnabled)
                .FirstOrDefaultAsync(ct);

            return pref ?? true;
        }

        private static void GenerateMonthlyCostsPdf(
            IReadOnlyList<ReportDTOs.MonthlyCostRow> rows,
            VehicleReportInfo vehicle,
            DateTime from,
            DateTime to,
            string path)
        {
            var total = rows.Sum(r => r.TotalAmount);
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Column(col =>
                    {
                        col.Item().Text($"Raport kosztów miesięcznych ({from:yyyy-MM-dd} - {to:yyyy-MM-dd})")
                            .FontSize(16).SemiBold();
                        col.Item().Text($"Pojazd: {vehicle.Brand} {vehicle.Model} ({vehicle.Year}), VIN: {vehicle.Vin}");
                        col.Item().Text($"Właściciel: {vehicle.OwnerName} ({vehicle.OwnerEmail ?? "brak e-mail"})");
                        col.Item().Text($"Status: {vehicle.Status}, Typ: {vehicle.Type}");
                        col.Item().Text($"Suma kosztów: {total:0.00} zł").SemiBold();
                    });
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Rok").SemiBold();
                            h.Cell().Text("Miesiąc").SemiBold();
                            h.Cell().Text("Kwota").SemiBold();
                            h.Cell().Text("Paliwo").SemiBold();
                            h.Cell().Text("Naprawy - robocizna").SemiBold();
                            h.Cell().Text("Naprawy - części").SemiBold();
                            h.Cell().Text("Pozostałe").SemiBold();
                            h.Cell().Text("Zdarzenia").SemiBold();
                        });

                        foreach (var r in rows)
                        {
                            table.Cell().Text(r.Year.ToString());
                            table.Cell().Text(GetMonthNamePl(r.Month));
                            table.Cell().Text(r.TotalAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.FuelAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.RepairLaborAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.RepairPartsAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.OtherAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.EventsCount.ToString());
                        }
                    });
                });
            }).GeneratePdf(path);
        }

        private static void GenerateCostsByTypePdf(
            IReadOnlyList<ReportDTOs.CostsByEventTypeRow> rows,
            VehicleReportInfo vehicle,
            DateTime from,
            DateTime to,
            string path)
        {
            var total = rows.Sum(r => r.TotalAmount);
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Column(col =>
                    {
                        col.Item().Text($"Raport kosztów wg typu zdarzenia ({from:yyyy-MM-dd} - {to:yyyy-MM-dd})")
                            .FontSize(16).SemiBold();
                        col.Item().Text($"Pojazd: {vehicle.Brand} {vehicle.Model} ({vehicle.Year}), VIN: {vehicle.Vin}");
                        col.Item().Text($"Właściciel: {vehicle.OwnerName} ({vehicle.OwnerEmail ?? "brak e-mail"})");
                        col.Item().Text($"Status: {vehicle.Status}, Typ: {vehicle.Type}");
                        col.Item().Text($"Suma kosztów: {total:0.00} zł").SemiBold();
                    });
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Data").SemiBold();
                            h.Cell().Text("Typ zdarzenia").SemiBold();
                            h.Cell().Text("Kwota").SemiBold();
                            h.Cell().Text("Paliwo").SemiBold();
                            h.Cell().Text("Naprawy - robocizna").SemiBold();
                            h.Cell().Text("Naprawy - części").SemiBold();
                            h.Cell().Text("Pozostałe").SemiBold();
                            h.Cell().Text("Zdarzenia").SemiBold();
                        });

                        foreach (var r in rows)
                        {
                            table.Cell().Text(r.EventDate.ToString("yyyy-MM-dd"));
                            table.Cell().Text(r.EventType);
                            table.Cell().Text(r.TotalAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.FuelAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.RepairLaborAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.RepairPartsAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.OtherAmount.ToString("0.00") + " zł");
                            table.Cell().Text(r.EventsCount.ToString());
                        }
                    });
                });
            }).GeneratePdf(path);
        }

        private static void GenerateRepairsHistoryPdf(
            IReadOnlyList<ReportDTOs.RepairHistoryRow> rows,
            VehicleReportInfo vehicle,
            DateTime from,
            DateTime to,
            string path)
        {
            var totalLabor = rows.Sum(r => r.LaborCost);
            var totalParts = rows.Sum(r => r.PartsCost);
            var total = totalLabor + totalParts;
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Column(col =>
                    {
                        col.Item().Text($"Raport historii napraw ({from:yyyy-MM-dd} - {to:yyyy-MM-dd})")
                            .FontSize(16).SemiBold();
                        col.Item().Text($"Pojazd: {vehicle.Brand} {vehicle.Model} ({vehicle.Year}), VIN: {vehicle.Vin}");
                        col.Item().Text($"Właściciel: {vehicle.OwnerName} ({vehicle.OwnerEmail ?? "brak e-mail"})");
                        col.Item().Text($"Status: {vehicle.Status}, Typ: {vehicle.Type}");
                        col.Item().Text($"Koszt robocizny: {totalLabor:0.00} zł | Części: {totalParts:0.00} zł | Razem: {total:0.00} zł")
                            .SemiBold();
                    });
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Data").SemiBold();
                            h.Cell().Text("Przebieg").SemiBold();
                            h.Cell().Text("Warsztat").SemiBold();
                            h.Cell().Text("Części").SemiBold();
                            h.Cell().Text("Robocizna").SemiBold();
                            h.Cell().Text("Części (koszt)").SemiBold();
                            h.Cell().Text("Razem").SemiBold();
                            h.Cell().Text("Opis").SemiBold();
                        });

                        foreach (var r in rows)
                        {
                            table.Cell().Text(r.EventDate.ToString("yyyy-MM-dd"));
                            table.Cell().Text(r.Mileage.ToString());
                            table.Cell().Text(string.IsNullOrWhiteSpace(r.WorkshopName) ? "-" : r.WorkshopName);
                            table.Cell().Text(string.IsNullOrWhiteSpace(r.PartsList) ? "-" : r.PartsList);
                            table.Cell().Text(r.LaborCost.ToString("0.00") + " zł");
                            table.Cell().Text(r.PartsCost.ToString("0.00") + " zł");
                            table.Cell().Text(r.TotalCost.ToString("0.00") + " zł");
                            table.Cell().Text(string.IsNullOrWhiteSpace(r.Description) ? "-" : r.Description);
                        }
                    });
                });
            }).GeneratePdf(path);
        }

        private static void GenerateMonthlyCostsExcel(
            IReadOnlyList<ReportDTOs.MonthlyCostRow> rows,
            VehicleReportInfo vehicle,
            DateTime from,
            DateTime to,
            string path)
        {
            var total = rows.Sum(r => r.TotalAmount);
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("KosztyMiesieczne");
            ws.Cell(1, 1).Value = "Raport kosztów miesięcznych";
            ws.Cell(2, 1).Value = "Zakres";
            ws.Cell(2, 2).Value = $"{from:yyyy-MM-dd} - {to:yyyy-MM-dd}";
            ws.Cell(3, 1).Value = "Pojazd";
            ws.Cell(3, 2).Value = $"{vehicle.Brand} {vehicle.Model} ({vehicle.Year}), VIN: {vehicle.Vin}";
            ws.Cell(4, 1).Value = "Właściciel";
            ws.Cell(4, 2).Value = $"{vehicle.OwnerName} ({vehicle.OwnerEmail ?? "brak e-mail"})";
            ws.Cell(5, 1).Value = "Suma kosztów";
            ws.Cell(5, 2).Value = total.ToString("0.00");

            ws.Cell(7, 1).Value = "Rok";
            ws.Cell(7, 2).Value = "Miesiąc";
            ws.Cell(7, 3).Value = "Kwota";
            ws.Cell(7, 4).Value = "Paliwo";
            ws.Cell(7, 5).Value = "Naprawy - robocizna";
            ws.Cell(7, 6).Value = "Naprawy - części";
            ws.Cell(7, 7).Value = "Pozostałe";
            ws.Cell(7, 8).Value = "Zdarzenia";

            var row = 8;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.Year;
                ws.Cell(row, 2).Value = GetMonthNamePl(r.Month);
                ws.Cell(row, 3).Value = r.TotalAmount;
                ws.Cell(row, 4).Value = r.FuelAmount;
                ws.Cell(row, 5).Value = r.RepairLaborAmount;
                ws.Cell(row, 6).Value = r.RepairPartsAmount;
                ws.Cell(row, 7).Value = r.OtherAmount;
                ws.Cell(row, 8).Value = r.EventsCount;
                row++;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(path);
        }

        private static string GetMonthNamePl(int month) => month switch
        {
            1 => "styczeń",
            2 => "luty",
            3 => "marzec",
            4 => "kwiecień",
            5 => "maj",
            6 => "czerwiec",
            7 => "lipiec",
            8 => "sierpień",
            9 => "wrzesień",
            10 => "październik",
            11 => "listopad",
            12 => "grudzień",
            _ => month.ToString()
        };

        private static void GenerateCostsByTypeExcel(
            IReadOnlyList<ReportDTOs.CostsByEventTypeRow> rows,
            VehicleReportInfo vehicle,
            DateTime from,
            DateTime to,
            string path)
        {
            var total = rows.Sum(r => r.TotalAmount);
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("KosztyWgTypu");
            ws.Cell(1, 1).Value = "Raport kosztów wg typu zdarzenia";
            ws.Cell(2, 1).Value = "Zakres";
            ws.Cell(2, 2).Value = $"{from:yyyy-MM-dd} - {to:yyyy-MM-dd}";
            ws.Cell(3, 1).Value = "Pojazd";
            ws.Cell(3, 2).Value = $"{vehicle.Brand} {vehicle.Model} ({vehicle.Year}), VIN: {vehicle.Vin}";
            ws.Cell(4, 1).Value = "Właściciel";
            ws.Cell(4, 2).Value = $"{vehicle.OwnerName} ({vehicle.OwnerEmail ?? "brak e-mail"})";
            ws.Cell(5, 1).Value = "Suma kosztów";
            ws.Cell(5, 2).Value = total.ToString("0.00");

            ws.Cell(7, 1).Value = "Typ zdarzenia";
            ws.Cell(7, 2).Value = "Data";
            ws.Cell(7, 3).Value = "Kwota";
            ws.Cell(7, 4).Value = "Paliwo";
            ws.Cell(7, 5).Value = "Naprawy - robocizna";
            ws.Cell(7, 6).Value = "Naprawy - części";
            ws.Cell(7, 7).Value = "Pozostałe";
            ws.Cell(7, 8).Value = "Zdarzenia";

            var row = 8;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.EventType;
                ws.Cell(row, 2).Value = r.EventDate.ToString("yyyy-MM-dd");
                ws.Cell(row, 3).Value = r.TotalAmount;
                ws.Cell(row, 4).Value = r.FuelAmount;
                ws.Cell(row, 5).Value = r.RepairLaborAmount;
                ws.Cell(row, 6).Value = r.RepairPartsAmount;
                ws.Cell(row, 7).Value = r.OtherAmount;
                ws.Cell(row, 8).Value = r.EventsCount;
                row++;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(path);
        }

        private static void GenerateRepairsHistoryExcel(
            IReadOnlyList<ReportDTOs.RepairHistoryRow> rows,
            VehicleReportInfo vehicle,
            DateTime from,
            DateTime to,
            string path)
        {
            var totalLabor = rows.Sum(r => r.LaborCost);
            var totalParts = rows.Sum(r => r.PartsCost);
            var total = totalLabor + totalParts;
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("HistoriaNapraw");
            ws.Cell(1, 1).Value = "Raport historii napraw";
            ws.Cell(2, 1).Value = "Zakres";
            ws.Cell(2, 2).Value = $"{from:yyyy-MM-dd} - {to:yyyy-MM-dd}";
            ws.Cell(3, 1).Value = "Pojazd";
            ws.Cell(3, 2).Value = $"{vehicle.Brand} {vehicle.Model} ({vehicle.Year}), VIN: {vehicle.Vin}";
            ws.Cell(4, 1).Value = "Właściciel";
            ws.Cell(4, 2).Value = $"{vehicle.OwnerName} ({vehicle.OwnerEmail ?? "brak e-mail"})";
            ws.Cell(5, 1).Value = "Robocizna";
            ws.Cell(5, 2).Value = totalLabor.ToString("0.00");
            ws.Cell(6, 1).Value = "Części";
            ws.Cell(6, 2).Value = totalParts.ToString("0.00");
            ws.Cell(7, 1).Value = "Razem";
            ws.Cell(7, 2).Value = total.ToString("0.00");

            ws.Cell(9, 1).Value = "Data";
            ws.Cell(9, 2).Value = "Przebieg";
            ws.Cell(9, 3).Value = "Warsztat";
            ws.Cell(9, 4).Value = "Części";
            ws.Cell(9, 5).Value = "Robocizna";
            ws.Cell(9, 6).Value = "Części (koszt)";
            ws.Cell(9, 7).Value = "Razem";
            ws.Cell(9, 8).Value = "Opis";

            var row = 10;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.EventDate.ToString("yyyy-MM-dd");
                ws.Cell(row, 2).Value = r.Mileage;
                ws.Cell(row, 3).Value = string.IsNullOrWhiteSpace(r.WorkshopName) ? "-" : r.WorkshopName;
                ws.Cell(row, 4).Value = string.IsNullOrWhiteSpace(r.PartsList) ? "-" : r.PartsList;
                ws.Cell(row, 5).Value = r.LaborCost;
                ws.Cell(row, 6).Value = r.PartsCost;
                ws.Cell(row, 7).Value = r.TotalCost;
                ws.Cell(row, 8).Value = string.IsNullOrWhiteSpace(r.Description) ? "-" : r.Description;
                row++;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(path);
        }

        public static RouteHandlerBuilder MapGetReports(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var reports = await reportsService.GetAllAsync(cancellationToken);

                return Results.Ok(reports);
            });
        }

        public static RouteHandlerBuilder MapGetReportById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var report = await reportsService.GetByIdAsync(id, cancellationToken);

                return report is null
                    ? Results.NotFound()
                    : Results.Ok(report);
            });
        }

        public static RouteHandlerBuilder MapCreateReport(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateReportDTO request,
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportsService.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/reports/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateReport(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateReportDTO request,
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportsService.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteReport(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IReportsService reportsService,
                CancellationToken cancellationToken) =>
            {
                var result = await reportsService.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
