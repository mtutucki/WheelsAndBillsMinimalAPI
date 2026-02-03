using WheelsAndBills.Application.DTOs.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.UserVehicles
{
    public interface IUserVehiclesService
    {
        Task<IReadOnlyList<GetVehiclesByUserDTO>> GetUserVehiclesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<VehicleDetailsDTO?> GetUserVehicleByIdAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default);
        Task<Guid> CreateVehicleAsync(Guid userId, CreateVehicleRequestDTO request, CancellationToken cancellationToken = default);
        Task<bool> DeleteVehicleAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default);
    }
}
