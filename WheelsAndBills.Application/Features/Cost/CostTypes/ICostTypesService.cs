using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Costs.CostsDTO;

namespace WheelsAndBills.Application.Features.Cost.CostTypes
{
    public interface ICostTypesService
    {
        Task<IReadOnlyList<GetCostTypeDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetCostTypeDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetCostTypeDTO>> CreateAsync(CreateCostTypeDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetCostTypeDTO>> UpdateAsync(Guid id, UpdateCostTypeDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
