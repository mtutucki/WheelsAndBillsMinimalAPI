using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.RepairEvents
{
    public interface IRepairEventsService
    {
        Task<IReadOnlyList<GetRepairEventDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetRepairEventDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetRepairEventDTO>> CreateAsync(CreateRepairEventDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetRepairEventDTO>> UpdateAsync(Guid id, UpdateRepairEventDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
