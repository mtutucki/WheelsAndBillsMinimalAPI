using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.EventParts
{
    public interface IEventPartsService
    {
        Task<IReadOnlyList<GetEventPartDTO>> GetByRepairEventAsync(Guid repairEventId, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetEventPartDTO>> CreateAsync(Guid repairEventId, CreateEventPartDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetEventPartDTO>> UpdateAsync(Guid repairEventId, Guid partId, UpdateEventPartDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid repairEventId, Guid partId, CancellationToken cancellationToken = default);
    }
}
