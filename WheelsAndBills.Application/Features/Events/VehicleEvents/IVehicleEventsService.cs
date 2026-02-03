using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Events;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.VehicleEvents
{
    public interface IVehicleEventsService
    {
        Task<IReadOnlyList<GetVehicleEventDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetVehicleEventDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleEventDTO>> CreateAsync(CreateVehicleEventDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleEventDTO>> UpdateAsync(Guid id, UpdateVehicleEventDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ServiceResult<GetVehicleEventDTO>> CreateForUserAsync(Guid userId, CreateMyVehicleEventDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteForUserAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    }
}
