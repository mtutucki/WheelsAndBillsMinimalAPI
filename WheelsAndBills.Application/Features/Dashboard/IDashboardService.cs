using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WheelsAndBills.Application.DTOs.Dashboard;

namespace WheelsAndBills.Application.Features.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardDto?> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<CostCompareResultDto> GetCostCompareAsync(Guid userId, string? range, CancellationToken cancellationToken = default);
    }
}
