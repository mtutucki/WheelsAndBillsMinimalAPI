using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Features.Reports.ReportParameters
{
    public interface IReportParametersService
    {
        Task<IReadOnlyList<GetReportParameterDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetReportParameterDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetReportParameterDTO>> CreateAsync(CreateReportParameterDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetReportParameterDTO>> UpdateAsync(Guid id, UpdateReportParameterDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
