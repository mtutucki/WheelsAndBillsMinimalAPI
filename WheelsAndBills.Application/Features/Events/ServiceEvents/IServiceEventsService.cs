using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.ServiceEvents
{
    public interface IServiceEventsService
    {
        Task<IReadOnlyList<GetServiceEventDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetServiceEventDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetServiceEventDTO>> CreateAsync(CreateServiceEventDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetServiceEventDTO>> UpdateAsync(Guid id, UpdateServiceEventDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
