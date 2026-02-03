using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehiclesAdmin
{
    public interface IVehiclesAdminService
    {
        Task<IReadOnlyList<GetVehicleDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetVehicleDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleDTO>> CreateAsync(CreateVehicleDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleDTO>> UpdateAsync(Guid id, UpdateVehicleDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
