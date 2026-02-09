using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.DTOs.Dashboard;

namespace WheelsAndBills.Application.Features.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IAppDbContext _db;
        public DashboardService(IAppDbContext db) 
        {
            _db = db;  
        }
        public async Task<DashboardDto?> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserSummaryDto(
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email!
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return null;

            var vehicles = await _db.Vehicles
                .Where(v => v.UserId == userId)
                .Select(v => new VehicleSummaryDto(
                    v.Id,
                    v.Brand.Name,
                    v.Model.Name,
                    v.Year,
                    v.Status.Name,
                    null
                ))
                .ToListAsync(cancellationToken);

            var latestMileage = await _db.VehicleMileage
                .Where(m => m.Vehicle.UserId == userId)
                .GroupBy(m => m.VehicleId)
                .Select(g => new { VehicleId = g.Key, Mileage = g.OrderByDescending(x => x.Date).First().Mileage })
                .ToDictionaryAsync(x => x.VehicleId, x => (int?)x.Mileage, cancellationToken);

            vehicles = vehicles.Select(v =>
                v with { CurrentMileage = latestMileage.TryGetValue(v.Id, out var m) ? m : null }
            ).ToList();

            var perVehicleFuelCost = await _db.FuelingEvents
                .Where(f => f.VehicleEvent.Vehicle.UserId == userId)
                .GroupBy(f => f.VehicleEvent.VehicleId)
                .Select(g => new { VehicleId = g.Key, Total = g.Sum(x => x.TotalPrice) })
                .ToDictionaryAsync(x => x.VehicleId, x => x.Total, cancellationToken);

            var perVehicleRepairCost = await _db.RepairEvents
                .Where(r => r.VehicleEvent.Vehicle.UserId == userId)
                .GroupBy(r => r.VehicleEvent.VehicleId)
                .Select(g => new { VehicleId = g.Key, Labor = g.Sum(x => x.LaborCost) })
                .ToDictionaryAsync(x => x.VehicleId, x => x.Labor, cancellationToken);

            var perVehiclePartsCost = await _db.EventParts
                .Where(p => p.RepairEvent.VehicleEvent.Vehicle.UserId == userId)
                .GroupBy(p => p.RepairEvent.VehicleEvent.VehicleId)
                .Select(g => new { VehicleId = g.Key, Parts = g.Sum(x => x.Price) })
                .ToDictionaryAsync(x => x.VehicleId, x => x.Parts, cancellationToken);

            var perVehicleTotalCost = new Dictionary<Guid, decimal>();

            foreach (var v in perVehicleFuelCost.Keys
                .Concat(perVehicleRepairCost.Keys)
                .Concat(perVehiclePartsCost.Keys)
                .Distinct())
            {
                perVehicleTotalCost[v] =
                    (perVehicleFuelCost.TryGetValue(v, out var fuel) ? fuel : 0m) +
                    (perVehicleRepairCost.TryGetValue(v, out var labor) ? labor : 0m) +
                    (perVehiclePartsCost.TryGetValue(v, out var parts) ? parts : 0m);
            }

            var totalSpentAllVehicles = perVehicleTotalCost.Values.Sum();

            var perVehicleLastFueling = await _db.FuelingEvents
                .Where(f => f.VehicleEvent.Vehicle.UserId == userId)
                .GroupBy(f => f.VehicleEvent.VehicleId)
                .Select(g => new
                {
                    VehicleId = g.Key,
                    LastFueling = g.Max(x => x.VehicleEvent.EventDate)
                })
                .ToDictionaryAsync(x => x.VehicleId, x => (DateTime?)x.LastFueling, cancellationToken);

            var lastFuelingDate = perVehicleLastFueling.Values
                .Where(d => d.HasValue)
                .Select(d => d!.Value)
                .OrderByDescending(d => d)
                .FirstOrDefault();

            var recurringMonthlyTotal = await _db.RecurringCosts
                .Where(r => r.Vehicle.UserId == userId)
                .SumAsync(r => r.Amount, cancellationToken);

            var newEventsCutoff = DateTime.UtcNow.Date.AddDays(-7);
            var newEventsCount = await _db.VehicleEvents
                .Where(e => e.Vehicle.UserId == userId && e.EventDate >= newEventsCutoff)
                .CountAsync(cancellationToken);

            var recentEvents = await _db.VehicleEvents
                .Where(e => e.Vehicle.UserId == userId)
                .OrderByDescending(e => e.EventDate)
                .Select(e => new
                {
                    e.VehicleId,
                    Event = new VehicleEventDto(
                        e.Id,
                        e.EventType.Name,
                        e.EventDate,
                        e.Mileage,
                        null
                    )
                })
                .ToListAsync(cancellationToken);

            var recentEventsByVehicle = recentEvents
                .GroupBy(x => x.VehicleId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Take(3).Select(x => x.Event).ToList()
                );

            var serviceTypeIds = await _db.EventTypes
                .Where(et =>
                    EF.Functions.Like(et.Name.ToLower(), "%serwis%") ||
                    EF.Functions.Like(et.Name.ToLower(), "%przegl%") ||
                    EF.Functions.Like(et.Name.ToLower(), "%service%") ||
                    EF.Functions.Like(et.Name.ToLower(), "%inspection%"))
                .Select(et => et.Id)
                .ToListAsync(cancellationToken);

            var lastServiceEvents = await _db.VehicleEvents
                .Where(e => e.Vehicle.UserId == userId && serviceTypeIds.Contains(e.EventTypeId))
                .GroupBy(e => e.VehicleId)
                .Select(g => g
                    .OrderByDescending(e => e.EventDate)
                    .Select(e => new { e.VehicleId, e.EventDate, e.Mileage })
                    .FirstOrDefault())
                .ToListAsync(cancellationToken);

            var lastServiceByVehicle = lastServiceEvents
                .Where(x => x != null)
                .ToDictionary(x => x!.VehicleId, x => x!);

            const int intervalKm = 10000;
            const int intervalMonths = 12;

            var serviceReminders = vehicles
                .Select(v =>
                {
                    lastServiceByVehicle.TryGetValue(v.Id, out var last);
                    var lastDate = last?.EventDate;
                    var lastMileage = last?.Mileage;
                    var nextDate = lastDate?.AddMonths(intervalMonths);
                    var nextMileage = lastMileage.HasValue ? lastMileage + intervalKm : null;
                    var currentMileage = latestMileage.TryGetValue(v.Id, out var m) ? m : null;

                    var overdueByDate = nextDate.HasValue &&
                                        DateTime.UtcNow.Date >= nextDate.Value.Date;
                    var overdueByMileage = currentMileage.HasValue &&
                                           nextMileage.HasValue &&
                                           currentMileage.Value >= nextMileage.Value;

                    return new ServiceReminderDto(
                        v.Id,
                        $"{v.Brand} {v.Model} ({v.Year})",
                        lastDate,
                        lastMileage,
                        nextDate,
                        nextMileage,
                        overdueByDate,
                        overdueByMileage
                    );
                })
                .ToList();

            return new DashboardDto(
                user,
                vehicles,
                new DashboardStatsDto(
                    vehicles.Count,
                    totalSpentAllVehicles,
                    recurringMonthlyTotal,
                    lastFuelingDate == default ? null : lastFuelingDate,
                    newEventsCount
                ),
                recentEventsByVehicle,
                perVehicleTotalCost,
                perVehicleLastFueling,
                serviceReminders
            );
        }

        public async Task<CostCompareResultDto> GetCostCompareAsync(
            Guid userId,
            string? range,
            CancellationToken cancellationToken = default)
        {
            var from = GetRangeStart(range);

            var vehicles = await _db.Vehicles
                .Where(v => v.UserId == userId)
                .Select(v => new
                {
                    v.Id,
                    v.BrandId,
                    v.ModelId,
                    Label = v.Brand.Name + " " + v.Model.Name + " (" + v.Year + ")"
                })
                .ToListAsync(cancellationToken);

            if (vehicles.Count == 0)
            {
                return new CostCompareResultDto(
                    range ?? "all",
                    new List<CostCompareVehicleDto>(),
                    new CostCompareAveragesDto(0m, 0m, 0m, 0m)
                );
            }

            var vehicleIds = vehicles.Select(v => v.Id).ToList();

            var allVehicles = await _db.Vehicles
                .Select(v => new
                {
                    v.Id,
                    v.BrandId,
                    v.ModelId
                })
                .ToListAsync(cancellationToken);

            var groupCounts = allVehicles
                .GroupBy(v => (v.BrandId, v.ModelId))
                .ToDictionary(g => g.Key, g => g.Count());

            var userFuelQuery =
                from f in _db.FuelingEvents
                join ve in _db.VehicleEvents on f.VehicleEventId equals ve.Id
                where vehicleIds.Contains(ve.VehicleId)
                select new { ve.VehicleId, ve.EventDate, f.TotalPrice };
            if (from.HasValue)
                userFuelQuery = userFuelQuery.Where(x => x.EventDate >= from.Value);

            var userRepairQuery =
                from r in _db.RepairEvents
                join ve in _db.VehicleEvents on r.VehicleEventId equals ve.Id
                where vehicleIds.Contains(ve.VehicleId)
                select new { ve.VehicleId, ve.EventDate, r.LaborCost };
            if (from.HasValue)
                userRepairQuery = userRepairQuery.Where(x => x.EventDate >= from.Value);

            var userPartsQuery =
                from p in _db.EventParts
                join r in _db.RepairEvents on p.RepairEventId equals r.Id
                join ve in _db.VehicleEvents on r.VehicleEventId equals ve.Id
                where vehicleIds.Contains(ve.VehicleId)
                select new { ve.VehicleId, ve.EventDate, p.Price };
            if (from.HasValue)
                userPartsQuery = userPartsQuery.Where(x => x.EventDate >= from.Value);

            var userFuel = await userFuelQuery
                .GroupBy(f => f.VehicleId)
                .Select(g => new { VehicleId = g.Key, Total = g.Sum(x => x.TotalPrice) })
                .ToListAsync(cancellationToken);

            var userRepairs = await userRepairQuery
                .GroupBy(r => r.VehicleId)
                .Select(g => new { VehicleId = g.Key, Total = g.Sum(x => x.LaborCost) })
                .ToListAsync(cancellationToken);

            var userParts = await userPartsQuery
                .GroupBy(p => p.VehicleId)
                .Select(g => new { VehicleId = g.Key, Total = g.Sum(x => x.Price) })
                .ToListAsync(cancellationToken);

            var fuelMap = userFuel.ToDictionary(x => x.VehicleId, x => x.Total);
            var repairsMap = userRepairs.ToDictionary(x => x.VehicleId, x => x.Total);
            var partsMap = userParts.ToDictionary(x => x.VehicleId, x => x.Total);

            var allFuelQuery =
                from f in _db.FuelingEvents
                join ve in _db.VehicleEvents on f.VehicleEventId equals ve.Id
                select new { ve.VehicleId, ve.EventDate, f.TotalPrice };
            if (from.HasValue)
                allFuelQuery = allFuelQuery.Where(x => x.EventDate >= from.Value);

            var allRepairQuery =
                from r in _db.RepairEvents
                join ve in _db.VehicleEvents on r.VehicleEventId equals ve.Id
                select new { ve.VehicleId, ve.EventDate, r.LaborCost };
            if (from.HasValue)
                allRepairQuery = allRepairQuery.Where(x => x.EventDate >= from.Value);

            var allPartsQuery =
                from p in _db.EventParts
                join r in _db.RepairEvents on p.RepairEventId equals r.Id
                join ve in _db.VehicleEvents on r.VehicleEventId equals ve.Id
                select new { ve.VehicleId, ve.EventDate, p.Price };
            if (from.HasValue)
                allPartsQuery = allPartsQuery.Where(x => x.EventDate >= from.Value);

            var allFuel = await allFuelQuery
                .GroupBy(f => f.VehicleId)
                .Select(g => new { VehicleId = g.Key, Total = g.Sum(x => x.TotalPrice) })
                .ToListAsync(cancellationToken);

            var allRepairs = await allRepairQuery
                .GroupBy(r => r.VehicleId)
                .Select(g => new { VehicleId = g.Key, Total = g.Sum(x => x.LaborCost) })
                .ToListAsync(cancellationToken);

            var allParts = await allPartsQuery
                .GroupBy(p => p.VehicleId)
                .Select(g => new { VehicleId = g.Key, Total = g.Sum(x => x.Price) })
                .ToListAsync(cancellationToken);

            var allFuelMap = allFuel.ToDictionary(x => x.VehicleId, x => x.Total);
            var allRepairsMap = allRepairs.ToDictionary(x => x.VehicleId, x => x.Total);
            var allPartsMap = allParts.ToDictionary(x => x.VehicleId, x => x.Total);

            var groupFuelSums = allVehicles
                .GroupBy(v => (v.BrandId, v.ModelId))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(v => allFuelMap.TryGetValue(v.Id, out var val) ? val : 0m)
                );

            var groupRepairsSums = allVehicles
                .GroupBy(v => (v.BrandId, v.ModelId))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(v => allRepairsMap.TryGetValue(v.Id, out var val) ? val : 0m)
                );

            var groupPartsSums = allVehicles
                .GroupBy(v => (v.BrandId, v.ModelId))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(v => allPartsMap.TryGetValue(v.Id, out var val) ? val : 0m)
                );

            var totalVehicles = allVehicles.Count;

            var sumFuel = await allFuelQuery
                .Select(f => (decimal?)f.TotalPrice)
                .SumAsync(cancellationToken) ?? 0m;

            var sumRepairs = await allRepairQuery
                .Select(r => (decimal?)r.LaborCost)
                .SumAsync(cancellationToken) ?? 0m;

            var sumParts = await allPartsQuery
                .Select(p => (decimal?)p.Price)
                .SumAsync(cancellationToken) ?? 0m;

            var avgFuel = totalVehicles > 0 ? sumFuel / totalVehicles : 0m;
            var avgRepairs = totalVehicles > 0 ? sumRepairs / totalVehicles : 0m;
            var avgParts = totalVehicles > 0 ? sumParts / totalVehicles : 0m;
            var avgTotal = avgFuel + avgRepairs + avgParts;

            var vehicleDtos = vehicles
                .Select(v =>
                {
                    var fuel = fuelMap.TryGetValue(v.Id, out var f) ? f : 0m;
                    var repairs = repairsMap.TryGetValue(v.Id, out var r) ? r : 0m;
                    var parts = partsMap.TryGetValue(v.Id, out var p) ? p : 0m;
                    var key = (v.BrandId, v.ModelId);
                    var count = groupCounts.TryGetValue(key, out var c) ? c : 0;
                    var avgFuelSame = count > 0 && groupFuelSums.TryGetValue(key, out var gf)
                        ? gf / count
                        : 0m;
                    var avgRepairsSame = count > 0 && groupRepairsSums.TryGetValue(key, out var gr)
                        ? gr / count
                        : 0m;
                    var avgPartsSame = count > 0 && groupPartsSums.TryGetValue(key, out var gp)
                        ? gp / count
                        : 0m;
                    var avgTotalSame = avgFuelSame + avgRepairsSame + avgPartsSame;
                    return new CostCompareVehicleDto(
                        v.Id,
                        v.Label,
                        fuel + repairs + parts,
                        fuel,
                        repairs,
                        parts,
                        avgTotalSame,
                        avgFuelSame,
                        avgRepairsSame,
                        avgPartsSame
                    );
                })
                .ToList();

            return new CostCompareResultDto(
                range ?? "all",
                vehicleDtos,
                new CostCompareAveragesDto(avgTotal, avgFuel, avgRepairs, avgParts)
            );
        }

        private static DateTime? GetRangeStart(string? range)
        {
            if (string.IsNullOrWhiteSpace(range) || range == "all")
                return null;

            if (int.TryParse(range, out var days) && days > 0)
                return DateTime.UtcNow.Date.AddDays(-days);

            return null;
        }
    }
}
