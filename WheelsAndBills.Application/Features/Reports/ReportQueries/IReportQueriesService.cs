using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Features.Reports.ReportQueries
{
    public interface IReportQueriesService
    {
        Task<IReadOnlyList<MonthlyCostRow>> GetMonthlyCostsAsync(Guid vehicleId, DateTime from, DateTime to, CancellationToken ct);
        Task<IReadOnlyList<CostsByEventTypeRow>> GetCostsByEventTypeAsync(Guid vehicleId, DateTime from, DateTime to, CancellationToken ct);
        Task<IReadOnlyList<RepairHistoryRow>> GetRepairHistoryAsync(Guid vehicleId, DateTime from, DateTime to, CancellationToken ct);
        Task<decimal> GetTotalCostAsync(Guid vehicleId, DateTime from, DateTime to, CancellationToken ct);
        Task<decimal> GetTotalRepairCostAsync(Guid vehicleId, DateTime from, DateTime to, CancellationToken ct);
    }
}
