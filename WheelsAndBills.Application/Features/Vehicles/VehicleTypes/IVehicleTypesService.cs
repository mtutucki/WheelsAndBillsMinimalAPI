using WheelsAndBills.Application.DTOs.Vehicles;
using WheelsAndBills.Application.Abstractions.Results;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleTypes
{
    public interface IVehicleTypesService
    {
        Task<IReadOnlyList<GetVehicleTypeDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetVehicleTypeDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleTypeDTO>> CreateAsync(CreateVehicleTypeDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleTypeDTO>> UpdateAsync(Guid id, UpdateVehicleTypeDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
