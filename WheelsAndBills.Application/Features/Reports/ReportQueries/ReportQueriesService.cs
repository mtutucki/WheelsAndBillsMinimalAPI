using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;
using WheelsAndBills.Application.Abstractions.Persistence;

namespace WheelsAndBills.Application.Features.Reports.ReportQueries;

public sealed class ReportQueriesService : IReportQueriesService
{
    private readonly IAppDbContext _db;

    public ReportQueriesService(IAppDbContext db)
    {
        _db = db;
    }

    // ================= PUBLIC API =================

    public Task<IReadOnlyList<MonthlyCostRow>> GetMonthlyCostsAsync(
        Guid vehicleId, DateTime from, DateTime to, CancellationToken ct)
        => ExecListAsync(
            "sp_Report_MonthlyCosts",
            vehicleId, from, to,
            r => new MonthlyCostRow
            {
                Year = GetInt32(r, "Year"),
                Month = GetInt32(r, "Month"),
                TotalAmount = GetDecimal(r, "TotalAmount"),
                FuelAmount = GetDecimalSafe(r, "FuelAmount"),
                RepairLaborAmount = GetDecimalSafe(r, "RepairLaborAmount"),
                RepairPartsAmount = GetDecimalSafe(r, "RepairPartsAmount"),
                OtherAmount = GetDecimalSafe(r, "OtherAmount"),
                EventsCount = GetInt32Safe(r, "EventsCount")
            },
            ct);

    public Task<IReadOnlyList<CostsByEventTypeRow>> GetCostsByEventTypeAsync(
        Guid vehicleId, DateTime from, DateTime to, CancellationToken ct)
        => ExecListAsync(
            "sp_Report_CostsByEventType",
            vehicleId, from, to,
            r => new CostsByEventTypeRow
            {
                EventDate = GetDateTimeSafe(r, "EventDate"),
                EventType = GetString(r, "EventType"),
                TotalAmount = GetDecimal(r, "TotalAmount"),
                FuelAmount = GetDecimalSafe(r, "FuelAmount"),
                RepairLaborAmount = GetDecimalSafe(r, "RepairLaborAmount"),
                RepairPartsAmount = GetDecimalSafe(r, "RepairPartsAmount"),
                OtherAmount = GetDecimalSafe(r, "OtherAmount"),
                EventsCount = GetInt32Safe(r, "EventsCount")
            },
            ct);

    public Task<IReadOnlyList<RepairHistoryRow>> GetRepairHistoryAsync(
        Guid vehicleId, DateTime from, DateTime to, CancellationToken ct)
        => ExecListAsync(
            "sp_Report_RepairsHistory",
            vehicleId, from, to,
            r => new RepairHistoryRow
            {
                EventDate = GetDateTime(r, "EventDate"),
                RepairEventId = GetGuid(r, "RepairEventId"),
                Mileage = GetInt32Safe(r, "Mileage"),
                Description = GetStringSafe(r, "Description"),
                WorkshopName = GetStringSafe(r, "WorkshopName"),
                PartsList = GetStringSafe(r, "PartsList"),
                LaborCost = GetDecimalSafe(r, "LaborCost"),
                PartsCost = GetDecimalSafe(r, "PartsCost"),
                TotalCost = GetDecimalSafe(r, "TotalCost")
            },
            ct);

    public Task<decimal> GetTotalCostAsync(
        Guid vehicleId, DateTime from, DateTime to, CancellationToken ct)
        => ExecScalarAsync(
            "SELECT dbo.fn_TotalCostForVehicle(@v,@f,@t)",
            vehicleId, from, to, ct);

    public Task<decimal> GetTotalRepairCostAsync(
        Guid vehicleId, DateTime from, DateTime to, CancellationToken ct)
        => ExecScalarAsync(
            "SELECT dbo.fn_TotalRepairCostForVehicle(@v,@f,@t)",
            vehicleId, from, to, ct);

    // ================= CORE HELPERS =================

    private async Task<IReadOnlyList<T>> ExecListAsync<T>(
        string storedProc,
        Guid vehicleId,
        DateTime from,
        DateTime to,
        Func<DbDataReader, T> map,
        CancellationToken ct)
    {
        var list = new List<T>();

        var connectionString = _db.Database.GetDbConnection().ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Server=localhost;Database=WheelsAndBillsAPI;Trusted_Connection=True;TrustServerCertificate=True";
        }

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = storedProc;
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add(new SqlParameter("@VehicleId", vehicleId));
        cmd.Parameters.Add(new SqlParameter("@DateFrom", from));
        cmd.Parameters.Add(new SqlParameter("@DateTo", to));

        await using var reader = await cmd.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
            list.Add(map(reader));

        return list;
    }

    private async Task<decimal> ExecScalarAsync(
        string sql,
        Guid vehicleId,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        var connectionString = _db.Database.GetDbConnection().ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Server=localhost;Database=WheelsAndBillsAPI;Trusted_Connection=True;TrustServerCertificate=True";
        }

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandType = CommandType.Text;

        cmd.Parameters.Add(new SqlParameter("@v", vehicleId));
        cmd.Parameters.Add(new SqlParameter("@f", from));
        cmd.Parameters.Add(new SqlParameter("@t", to));

        var result = await cmd.ExecuteScalarAsync(ct);
        return result == DBNull.Value || result is null
            ? 0m
            : Convert.ToDecimal(result);
    }

    private static int GetInt32(DbDataReader r, string name)
        => r.GetInt32(r.GetOrdinal(name));

    private static decimal GetDecimal(DbDataReader r, string name)
        => r.GetDecimal(r.GetOrdinal(name));

    private static bool TryGetOrdinal(DbDataReader r, string name, out int ordinal)
    {
        for (var i = 0; i < r.FieldCount; i++)
        {
            if (string.Equals(r.GetName(i), name, StringComparison.OrdinalIgnoreCase))
            {
                ordinal = i;
                return true;
            }
        }

        ordinal = -1;
        return false;
    }

    private static int GetInt32Safe(DbDataReader r, string name)
    {
        if (!TryGetOrdinal(r, name, out var ordinal))
            return 0;

        var value = r.GetValue(ordinal);
        return value == DBNull.Value || value is null
            ? 0
            : Convert.ToInt32(value);
    }

    private static decimal GetDecimalSafe(DbDataReader r, string name)
    {
        if (!TryGetOrdinal(r, name, out var ordinal))
            return 0m;

        var value = r.GetValue(ordinal);
        return value == DBNull.Value || value is null
            ? 0m
            : Convert.ToDecimal(value);
    }

    private static string? GetStringSafe(DbDataReader r, string name)
    {
        if (!TryGetOrdinal(r, name, out var ordinal))
            return null;

        var value = r.GetValue(ordinal);
        return value == DBNull.Value || value is null
            ? null
            : Convert.ToString(value);
    }

    private static DateTime GetDateTimeSafe(DbDataReader r, string name)
    {
        if (!TryGetOrdinal(r, name, out var ordinal))
            return default;

        var value = r.GetValue(ordinal);
        return value == DBNull.Value || value is null
            ? default
            : Convert.ToDateTime(value);
    }

    private static string GetString(DbDataReader r, string name)
        => r.GetString(r.GetOrdinal(name));

    private static DateTime GetDateTime(DbDataReader r, string name)
        => r.GetDateTime(r.GetOrdinal(name));

    private static Guid GetGuid(DbDataReader r, string name)
        => r.GetGuid(r.GetOrdinal(name));
}
