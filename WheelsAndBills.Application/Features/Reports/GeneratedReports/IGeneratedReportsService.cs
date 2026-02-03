using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Features.Reports.GeneratedReports
{
    public interface IGeneratedReportsService
    {
        Task<IReadOnlyList<GetGeneratedReportDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetGeneratedReportDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetGeneratedReportDTO>> CreateAsync(CreateGeneratedReportDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetGeneratedReportDTO>> UpdateAsync(Guid id, UpdateGeneratedReportDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
