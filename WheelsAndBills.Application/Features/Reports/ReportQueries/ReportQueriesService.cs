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
                TotalAmount = GetDecimal(r, "TotalAmount")
            },
            ct);

    public Task<IReadOnlyList<CostsByEventTypeRow>> GetCostsByEventTypeAsync(
        Guid vehicleId, DateTime from, DateTime to, CancellationToken ct)
        => ExecListAsync(
            "sp_Report_CostsByEventType",
            vehicleId, from, to,
            r => new CostsByEventTypeRow
            {
                EventType = GetString(r, "EventType"),
                TotalAmount = GetDecimal(r, "TotalAmount")
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
                LaborCost = GetDecimal(r, "LaborCost"),
                PartsCost = GetDecimal(r, "PartsCost")
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

        await using var conn = _db.Database.GetDbConnection();
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
        await using var conn = _db.Database.GetDbConnection();
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

    private static string GetString(DbDataReader r, string name)
        => r.GetString(r.GetOrdinal(name));

    private static DateTime GetDateTime(DbDataReader r, string name)
        => r.GetDateTime(r.GetOrdinal(name));

    private static Guid GetGuid(DbDataReader r, string name)
        => r.GetGuid(r.GetOrdinal(name));
}
