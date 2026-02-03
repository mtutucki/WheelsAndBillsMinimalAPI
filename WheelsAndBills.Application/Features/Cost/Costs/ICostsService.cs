using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Costs.CostsDTO;

namespace WheelsAndBills.Application.Features.Cost.Costs
{
    public interface ICostsService
    {
        Task<IReadOnlyList<GetCostDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetCostDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetCostDTO>> CreateAsync(CreateCostDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetCostDTO>> UpdateAsync(Guid id, UpdateCostDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
