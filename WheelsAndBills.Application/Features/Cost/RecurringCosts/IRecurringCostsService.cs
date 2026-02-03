using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Costs.CostsDTO;

namespace WheelsAndBills.Application.Features.Cost.RecurringCosts
{
    public interface IRecurringCostsService
    {
        Task<IReadOnlyList<GetRecurringCostDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetRecurringCostDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetRecurringCostDTO>> CreateAsync(CreateRecurringCostDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetRecurringCostDTO>> UpdateAsync(Guid id, UpdateRecurringCostDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
