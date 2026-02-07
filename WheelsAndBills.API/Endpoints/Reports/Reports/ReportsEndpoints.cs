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
                            .ToList()
                    ))
                    .ToListAsync(cancellationToken);

                return Results.Ok(items);
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

                var reportsRoot = Path.Combine(env.ContentRootPath, "ReportsFiles");
                Directory.CreateDirectory(reportsRoot);

                var baseName = $"{report.DefinitionCode}_{report.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
                var pdfPath = Path.Combine(reportsRoot, $"{baseName}.pdf");
                var xlsxPath = Path.Combine(reportsRoot, $"{baseName}.xlsx");

                if (report.DefinitionCode == "MONTHLY_COSTS")
                {
                    var rows = await reportQueries.GetMonthlyCostsAsync(vehicleId, from, to, cancellationToken);
                    GenerateMonthlyCostsPdf(rows, report.DefinitionCode, from, to, pdfPath);
                    GenerateMonthlyCostsExcel(rows, report.DefinitionCode, from, to, xlsxPath);
                }
                else if (report.DefinitionCode == "COSTS_BY_EVENT_TYPE")
                {
                    var rows = await reportQueries.GetCostsByEventTypeAsync(vehicleId, from, to, cancellationToken);
                    GenerateCostsByTypePdf(rows, report.DefinitionCode, from, to, pdfPath);
                    GenerateCostsByTypeExcel(rows, report.DefinitionCode, from, to, xlsxPath);
                }
                else if (report.DefinitionCode == "REPAIRS_HISTORY")
                {
                    var rows = await reportQueries.GetRepairHistoryAsync(vehicleId, from, to, cancellationToken);
                    GenerateRepairsHistoryPdf(rows, report.DefinitionCode, from, to, pdfPath);
                    GenerateRepairsHistoryExcel(rows, report.DefinitionCode, from, to, xlsxPath);
                }
                else
                {
                    return Results.BadRequest("Unsupported report definition");
                }

                db.GeneratedReports.Add(new WheelsAndBills.Domain.Entities.Report.GeneratedReport
                {
                    Id = Guid.NewGuid(),
                    ReportId = report.Id,
                    FilePath = pdfPath
                });

                db.GeneratedReports.Add(new WheelsAndBills.Domain.Entities.Report.GeneratedReport
                {
                    Id = Guid.NewGuid(),
                    ReportId = report.Id,
                    FilePath = xlsxPath
                });

                await db.SaveChangesAsync(cancellationToken);

                return Results.Ok(new { PdfPath = pdfPath, ExcelPath = xlsxPath });
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

        private static void GenerateMonthlyCostsPdf(
            IReadOnlyList<ReportDTOs.MonthlyCostRow> rows,
            string title,
            DateTime from,
            DateTime to,
            string path)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"{title} ({from:yyyy-MM-dd} - {to:yyyy-MM-dd})").FontSize(16).SemiBold();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Year").SemiBold();
                            h.Cell().Text("Month").SemiBold();
                            h.Cell().Text("TotalAmount").SemiBold();
                        });

                        foreach (var r in rows)
                        {
                            table.Cell().Text(r.Year.ToString());
                            table.Cell().Text(r.Month.ToString());
                            table.Cell().Text(r.TotalAmount.ToString("0.00"));
                        }
                    });
                });
            }).GeneratePdf(path);
        }

        private static void GenerateCostsByTypePdf(
            IReadOnlyList<ReportDTOs.CostsByEventTypeRow> rows,
            string title,
            DateTime from,
            DateTime to,
            string path)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"{title} ({from:yyyy-MM-dd} - {to:yyyy-MM-dd})").FontSize(16).SemiBold();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("EventType").SemiBold();
                            h.Cell().Text("TotalAmount").SemiBold();
                        });

                        foreach (var r in rows)
                        {
                            table.Cell().Text(r.EventType);
                            table.Cell().Text(r.TotalAmount.ToString("0.00"));
                        }
                    });
                });
            }).GeneratePdf(path);
        }

        private static void GenerateRepairsHistoryPdf(
            IReadOnlyList<ReportDTOs.RepairHistoryRow> rows,
            string title,
            DateTime from,
            DateTime to,
            string path)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"{title} ({from:yyyy-MM-dd} - {to:yyyy-MM-dd})").FontSize(16).SemiBold();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("EventDate").SemiBold();
                            h.Cell().Text("RepairEventId").SemiBold();
                            h.Cell().Text("LaborCost").SemiBold();
                            h.Cell().Text("PartsCost").SemiBold();
                        });

                        foreach (var r in rows)
                        {
                            table.Cell().Text(r.EventDate.ToString("yyyy-MM-dd"));
                            table.Cell().Text(r.RepairEventId.ToString());
                            table.Cell().Text(r.LaborCost.ToString("0.00"));
                            table.Cell().Text(r.PartsCost.ToString("0.00"));
                        }
                    });
                });
            }).GeneratePdf(path);
        }

        private static void GenerateMonthlyCostsExcel(
            IReadOnlyList<ReportDTOs.MonthlyCostRow> rows,
            string title,
            DateTime from,
            DateTime to,
            string path)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("MonthlyCosts");
            ws.Cell(1, 1).Value = title;
            ws.Cell(2, 1).Value = "From";
            ws.Cell(2, 2).Value = from.ToString("yyyy-MM-dd");
            ws.Cell(3, 1).Value = "To";
            ws.Cell(3, 2).Value = to.ToString("yyyy-MM-dd");

            ws.Cell(5, 1).Value = "Year";
            ws.Cell(5, 2).Value = "Month";
            ws.Cell(5, 3).Value = "TotalAmount";

            var row = 6;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.Year;
                ws.Cell(row, 2).Value = r.Month;
                ws.Cell(row, 3).Value = r.TotalAmount;
                row++;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(path);
        }

        private static void GenerateCostsByTypeExcel(
            IReadOnlyList<ReportDTOs.CostsByEventTypeRow> rows,
            string title,
            DateTime from,
            DateTime to,
            string path)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("CostsByEventType");
            ws.Cell(1, 1).Value = title;
            ws.Cell(2, 1).Value = "From";
            ws.Cell(2, 2).Value = from.ToString("yyyy-MM-dd");
            ws.Cell(3, 1).Value = "To";
            ws.Cell(3, 2).Value = to.ToString("yyyy-MM-dd");

            ws.Cell(5, 1).Value = "EventType";
            ws.Cell(5, 2).Value = "TotalAmount";

            var row = 6;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.EventType;
                ws.Cell(row, 2).Value = r.TotalAmount;
                row++;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(path);
        }

        private static void GenerateRepairsHistoryExcel(
            IReadOnlyList<ReportDTOs.RepairHistoryRow> rows,
            string title,
            DateTime from,
            DateTime to,
            string path)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("RepairsHistory");
            ws.Cell(1, 1).Value = title;
            ws.Cell(2, 1).Value = "From";
            ws.Cell(2, 2).Value = from.ToString("yyyy-MM-dd");
            ws.Cell(3, 1).Value = "To";
            ws.Cell(3, 2).Value = to.ToString("yyyy-MM-dd");

            ws.Cell(5, 1).Value = "EventDate";
            ws.Cell(5, 2).Value = "RepairEventId";
            ws.Cell(5, 3).Value = "LaborCost";
            ws.Cell(5, 4).Value = "PartsCost";

            var row = 6;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.EventDate.ToString("yyyy-MM-dd");
                ws.Cell(row, 2).Value = r.RepairEventId.ToString();
                ws.Cell(row, 3).Value = r.LaborCost;
                ws.Cell(row, 4).Value = r.PartsCost;
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
