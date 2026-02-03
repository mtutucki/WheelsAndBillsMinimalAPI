using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleStatuses
{
    public interface IVehicleStatusesService
    {
        Task<IReadOnlyList<GetVehicleStatusDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetVehicleStatusDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleStatusDTO>> CreateAsync(CreateVehicleStatusDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleStatusDTO>> UpdateAsync(Guid id, UpdateVehicleStatusDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
