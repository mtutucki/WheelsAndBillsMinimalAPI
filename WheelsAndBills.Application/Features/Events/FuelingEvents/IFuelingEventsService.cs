using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.FuelingEvents
{
    public interface IFuelingEventsService
    {
        Task<IReadOnlyList<GetFuelingEventDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetFuelingEventDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetFuelingEventDTO>> CreateAsync(CreateFuelingEventDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetFuelingEventDTO>> UpdateAsync(Guid id, UpdateFuelingEventDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
