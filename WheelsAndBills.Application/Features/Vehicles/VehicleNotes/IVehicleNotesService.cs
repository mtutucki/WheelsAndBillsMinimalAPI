using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleNotes
{
    public interface IVehicleNotesService
    {
        Task<IReadOnlyList<GetVehicleNoteDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetVehicleNoteDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleNoteDTO>> CreateAsync(CreateVehicleNoteDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetVehicleNoteDTO>> UpdateAsync(Guid id, UpdateVehicleNoteDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ServiceResult<GetVehicleNoteDTO>> CreateForUserAsync(Guid userId, CreateMyVehicleNoteDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteForUserAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    }
}
