using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.EventTypes
{
    public interface IEventTypesService
    {
        Task<IReadOnlyList<GetEventTypeDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetEventTypeDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetEventTypeDTO>> CreateAsync(CreateEventTypeDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetEventTypeDTO>> UpdateAsync(Guid id, UpdateEventTypeDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
