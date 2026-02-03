using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleMileage
{
    public interface IVehicleMileageService
    {
        Task<IReadOnlyList<GetVehicleMileageDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetVehicleMileageDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleMileageDTO>> CreateAsync(CreateVehicleMileageDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleMileageDTO>> UpdateAsync(Guid id, UpdateVehicleMileageDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ServiceResult<GetVehicleMileageDTO>> CreateForUserAsync(Guid userId, CreateVehicleMileageDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteForUserAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    }
}
