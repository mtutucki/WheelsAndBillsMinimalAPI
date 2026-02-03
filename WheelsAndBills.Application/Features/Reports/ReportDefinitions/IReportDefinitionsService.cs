using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Features.Reports.ReportDefinitions
{
    public interface IReportDefinitionsService
    {
        Task<IReadOnlyList<GetReportDefinitionDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetReportDefinitionDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetReportDefinitionDTO>> CreateAsync(CreateReportDefinitionDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetReportDefinitionDTO>> UpdateAsync(Guid id, UpdateReportDefinitionDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
