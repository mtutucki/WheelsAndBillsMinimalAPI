using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Features.Reports.Reports
{
    public interface IReportsService
    {
        Task<IReadOnlyList<GetReportDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetReportDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetReportDTO>> CreateAsync(CreateReportDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetReportDTO>> UpdateAsync(Guid id, UpdateReportDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
