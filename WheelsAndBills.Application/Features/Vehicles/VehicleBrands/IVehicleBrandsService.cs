using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleBrands
{
    public interface IVehicleBrandsService
    {
        Task<IReadOnlyList<GetVehicleBrandDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetVehicleBrandDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleBrandDTO>> CreateAsync(CreateVehicleBrandDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleBrandDTO>> UpdateAsync(Guid id, UpdateVehicleBrandDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
