using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Features.Reports.ReportQueries;
using WheelsAndBills.Domain.Entities.Report;

namespace WheelsAndBills.API.Endpoints.Reports.ReportQueries
{
    public static class ReportQueries
    {
        private const string CodeMonthlyCosts = "MONTHLY_COSTS";
        private const string CodeCostsByEventType = "COSTS_BY_EVENT_TYPE";
        private const string CodeRepairsHistory = "REPAIRS_HISTORY";

        public static IEndpointRouteBuilder MapReportGeneration(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/reports/generate").RequireAuthorization();

            group.MapGet("/monthly-costs", async (
                ClaimsPrincipal user,
                [FromQuery] Guid vehicleId,
                [FromQuery] DateTime from,
                [FromQuery] DateTime to,
                [FromServices] IReportQueriesService service,
                [FromServices] IAppDbContext db,
                CancellationToken ct) =>
            {
                var userId = GetUserId(user);
                if (userId is null) return Results.Unauthorized();

                var defId = await GetOrCreateDefinitionIdAsync(db, CodeMonthlyCosts, ct);
                await CreateReportAsync(db, userId.Value, defId, vehicleId, from, to, ct);

                var rows = await service.GetMonthlyCostsAsync(vehicleId, from, to, ct);
                return Results.Ok(rows);
            });

            group.MapGet("/costs-by-event-type", async (
                ClaimsPrincipal user,
                [FromQuery] Guid vehicleId,
                [FromQuery] DateTime from,
                [FromQuery] DateTime to,
                [FromServices] IReportQueriesService service,
                [FromServices] IAppDbContext db,
                CancellationToken ct) =>
            {
                var userId = GetUserId(user);
                if (userId is null) return Results.Unauthorized();

                var defId = await GetOrCreateDefinitionIdAsync(db, CodeCostsByEventType, ct);
                await CreateReportAsync(db, userId.Value, defId, vehicleId, from, to, ct);

                var rows = await service.GetCostsByEventTypeAsync(vehicleId, from, to, ct);
                return Results.Ok(rows);
            });

            group.MapGet("/repairs-history", async (
                ClaimsPrincipal user,
                [FromQuery] Guid vehicleId,
                [FromQuery] DateTime from,
                [FromQuery] DateTime to,
                [FromServices] IReportQueriesService service,
                [FromServices] IAppDbContext db,
                CancellationToken ct) =>
            {
                var userId = GetUserId(user);
                if (userId is null) return Results.Unauthorized();

                var defId = await GetOrCreateDefinitionIdAsync(db, CodeRepairsHistory, ct);
                await CreateReportAsync(db, userId.Value, defId, vehicleId, from, to, ct);

                var rows = await service.GetRepairHistoryAsync(vehicleId, from, to, ct);
                return Results.Ok(rows);
            });

            group.MapGet("/total-cost", async (
                ClaimsPrincipal user,
                [FromQuery] Guid vehicleId,
                [FromQuery] DateTime from,
                [FromQuery] DateTime to,
                [FromServices] IReportQueriesService service,
                [FromServices] IAppDbContext db,
                CancellationToken ct) =>
            {
                var userId = GetUserId(user);
                if (userId is null) return Results.Unauthorized();

                var defId = await GetOrCreateDefinitionIdAsync(db, CodeMonthlyCosts, ct);
                await CreateReportAsync(db, userId.Value, defId, vehicleId, from, to, ct);

                var total = await service.GetTotalCostAsync(vehicleId, from, to, ct);
                return Results.Ok(total);
            });

            group.MapGet("/total-repair-cost", async (
                ClaimsPrincipal user,
                [FromQuery] Guid vehicleId,
                [FromQuery] DateTime from,
                [FromQuery] DateTime to,
                [FromServices] IReportQueriesService service,
                [FromServices] IAppDbContext db,
                CancellationToken ct) =>
            {
                var userId = GetUserId(user);
                if (userId is null) return Results.Unauthorized();

                var defId = await GetOrCreateDefinitionIdAsync(db, CodeRepairsHistory, ct);
                await CreateReportAsync(db, userId.Value, defId, vehicleId, from, to, ct);

                var total = await service.GetTotalRepairCostAsync(vehicleId, from, to, ct);
                return Results.Ok(total);
            });

            return app;
        }

        private static Guid? GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return null;

            return userId;
        }

        private static async Task<Guid> GetOrCreateDefinitionIdAsync(
            IAppDbContext db,
            string code,
            CancellationToken ct)
        {
            var def = await db.ReportDefinitions
                .FirstOrDefaultAsync(d => d.Code == code, ct);

            if (def is not null) return def.Id;

            def = new ReportDefinition
            {
                Id = Guid.NewGuid(),
                Code = code
            };

            db.ReportDefinitions.Add(def);
            await db.SaveChangesAsync(ct);

            return def.Id;
        }

        private static async Task CreateReportAsync(
            IAppDbContext db,
            Guid userId,
            Guid reportDefinitionId,
            Guid vehicleId,
            DateTime from,
            DateTime to,
            CancellationToken ct)
        {
            var report = new Report
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReportDefinitionId = reportDefinitionId,
                CreatedAt = DateTime.UtcNow
            };

            db.Reports.Add(report);

            db.ReportParameters.AddRange(new[]
            {
                new ReportParameter
                {
                    Id = Guid.NewGuid(),
                    ReportId = report.Id,
                    Name = "vehicleId",
                    Value = vehicleId.ToString()
                },
                new ReportParameter
                {
                    Id = Guid.NewGuid(),
                    ReportId = report.Id,
                    Name = "from",
                    Value = from.ToString("O")
                },
                new ReportParameter
                {
                    Id = Guid.NewGuid(),
                    ReportId = report.Id,
                    Name = "to",
                    Value = to.ToString("O")
                }
            });

            await db.SaveChangesAsync(ct);
        }
    }
}
