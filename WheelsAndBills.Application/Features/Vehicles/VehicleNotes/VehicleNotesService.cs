using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleNotes
{
    public class VehicleNotesService : IVehicleNotesService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorForbidden = "Forbidden";
        private const string ErrorVehicleMissing = "Vehicle does not exist";
        private const string ErrorUserMissing = "User does not exist";
        private const string ErrorVehicleNotOwned = "Vehicle does not belong to user";

        private readonly IAppDbContext _db;

        public VehicleNotesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetVehicleNoteDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.VehicleNotes
                .Select(n => new GetVehicleNoteDTO(
                    n.Id,
                    n.VehicleId,
                    n.UserId,
                    n.Content,
                    n.CreatedAt
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetVehicleNoteDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.VehicleNotes
                .Where(n => n.Id == id)
                .Select(n => new GetVehicleNoteDTO(
                    n.Id,
                    n.VehicleId,
                    n.UserId,
                    n.Content,
                    n.CreatedAt
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetVehicleNoteDTO>> CreateAsync(CreateVehicleNoteDTO request, CancellationToken cancellationToken = default)
        {
            var vehicleExists = await _db.Vehicles
                .AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
            if (!vehicleExists)
                return ServiceResult<GetVehicleNoteDTO>.Fail(ErrorVehicleMissing);

            var userExists = await _db.Users
                .AnyAsync(u => u.Id == request.UserId, cancellationToken);
            if (!userExists)
                return ServiceResult<GetVehicleNoteDTO>.Fail(ErrorUserMissing);

            var note = new VehicleNote
            {
                Id = Guid.NewGuid(),
                VehicleId = request.VehicleId,
                UserId = request.UserId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _db.VehicleNotes.Add(note);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleNoteDTO>.Ok(new GetVehicleNoteDTO(
                note.Id,
                note.VehicleId,
                note.UserId,
                note.Content,
                note.CreatedAt
            ));
        }

        public async Task<ServiceResult<GetVehicleNoteDTO>> UpdateAsync(Guid id, UpdateVehicleNoteDTO request, CancellationToken cancellationToken = default)
        {
            var note = await _db.VehicleNotes.FindAsync(new object?[] { id }, cancellationToken);
            if (note is null)
                return ServiceResult<GetVehicleNoteDTO>.Fail(ErrorNotFound);

            note.Content = request.Content;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleNoteDTO>.Ok(new GetVehicleNoteDTO(
                note.Id,
                note.VehicleId,
                note.UserId,
                note.Content,
                note.CreatedAt
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var note = await _db.VehicleNotes.FindAsync(new object?[] { id }, cancellationToken);
            if (note is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.VehicleNotes.Remove(note);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<GetVehicleNoteDTO>> CreateForUserAsync(Guid userId, CreateMyVehicleNoteDTO request, CancellationToken cancellationToken = default)
        {
            var vehicleExists = await _db.Vehicles
                .AnyAsync(v => v.Id == request.VehicleId && v.UserId == userId, cancellationToken);

            if (!vehicleExists)
                return ServiceResult<GetVehicleNoteDTO>.Fail(ErrorVehicleNotOwned);

            var note = new VehicleNote
            {
                Id = Guid.NewGuid(),
                VehicleId = request.VehicleId,
                UserId = userId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _db.VehicleNotes.Add(note);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleNoteDTO>.Ok(new GetVehicleNoteDTO(
                note.Id,
                note.VehicleId,
                note.UserId,
                note.Content,
                note.CreatedAt
            ));
        }

        public async Task<ServiceResult> DeleteForUserAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _db.VehicleNotes
                .Include(vm => vm.Vehicle)
                .FirstOrDefaultAsync(vm => vm.Id == id, cancellationToken);

            if (item is null)
                return ServiceResult.Fail(ErrorNotFound);

            if (item.Vehicle.UserId != userId)
                return ServiceResult.Fail(ErrorForbidden);

            _db.VehicleNotes.Remove(item);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
