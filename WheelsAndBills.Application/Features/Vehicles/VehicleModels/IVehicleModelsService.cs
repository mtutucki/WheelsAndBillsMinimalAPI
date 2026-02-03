using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleModels
{
    public interface IVehicleModelsService
    {
        Task<IReadOnlyList<GetVehicleModelDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetVehicleModelDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleModelDTO>> CreateAsync(CreateVehicleModelDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleModelDTO>> UpdateAsync(Guid id, UpdateVehicleModelDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
