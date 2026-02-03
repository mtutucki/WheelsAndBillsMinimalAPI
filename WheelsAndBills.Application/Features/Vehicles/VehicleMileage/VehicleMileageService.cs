using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;
using VehicleMileageEntity = WheelsAndBills.Domain.Entities.Vehicles.VehicleMileage;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleMileage
{
    public class VehicleMileageService : IVehicleMileageService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorForbidden = "Forbidden";
        private const string ErrorVehicleMissing = "Vehicle does not exist";
        private const string ErrorVehicleNotOwned = "Vehicle does not belong to user";

        private readonly IAppDbContext _db;

        public VehicleMileageService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetVehicleMileageDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.VehicleMileage
                .Select(m => new GetVehicleMileageDTO(
                    m.Id,
                    m.VehicleId,
                    m.Mileage,
                    m.Date
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetVehicleMileageDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.VehicleMileage
                .Where(m => m.Id == id)
                .Select(m => new GetVehicleMileageDTO(
                    m.Id,
                    m.VehicleId,
                    m.Mileage,
                    m.Date
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetVehicleMileageDTO>> CreateAsync(CreateVehicleMileageDTO request, CancellationToken cancellationToken = default)
        {
            var vehicleExists = await _db.Vehicles
                .AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
            if (!vehicleExists)
                return ServiceResult<GetVehicleMileageDTO>.Fail(ErrorVehicleMissing);

            var item = new VehicleMileageEntity
            {
                Id = Guid.NewGuid(),
                VehicleId = request.VehicleId,
                Mileage = request.Mileage,
                Date = request.Date
            };

            _db.VehicleMileage.Add(item);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleMileageDTO>.Ok(new GetVehicleMileageDTO(
                item.Id,
                item.VehicleId,
                item.Mileage,
                item.Date
            ));
        }

        public async Task<ServiceResult<GetVehicleMileageDTO>> UpdateAsync(Guid id, UpdateVehicleMileageDTO request, CancellationToken cancellationToken = default)
        {
            var item = await _db.VehicleMileage.FindAsync(new object?[] { id }, cancellationToken);
            if (item is null)
                return ServiceResult<GetVehicleMileageDTO>.Fail(ErrorNotFound);

            item.Mileage = request.Mileage;
            item.Date = request.Date;

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleMileageDTO>.Ok(new GetVehicleMileageDTO(
                item.Id,
                item.VehicleId,
                item.Mileage,
                item.Date
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _db.VehicleMileage.FindAsync(new object?[] { id }, cancellationToken);
            if (item is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.VehicleMileage.Remove(item);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<GetVehicleMileageDTO>> CreateForUserAsync(Guid userId, CreateVehicleMileageDTO request, CancellationToken cancellationToken = default)
        {
            var vehicleExists = await _db.Vehicles
                .AnyAsync(v =>
                    v.Id == request.VehicleId &&
                    v.UserId == userId, cancellationToken);

            if (!vehicleExists)
                return ServiceResult<GetVehicleMileageDTO>.Fail(ErrorVehicleNotOwned);

            var lastMileage = await _db.VehicleMileage
                .Where(x => x.VehicleId == request.VehicleId)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Mileage)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastMileage != 0 && request.Mileage <= lastMileage)
                return ServiceResult<GetVehicleMileageDTO>.Fail(
                    $"Mileage must be greater than last value ({lastMileage} km)"
                );

            var item = new VehicleMileageEntity
            {
                Id = Guid.NewGuid(),
                VehicleId = request.VehicleId,
                Mileage = request.Mileage,
                Date = request.Date
            };

            _db.VehicleMileage.Add(item);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleMileageDTO>.Ok(new GetVehicleMileageDTO(
                item.Id,
                item.VehicleId,
                item.Mileage,
                item.Date
            ));
        }

        public async Task<ServiceResult> DeleteForUserAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _db.VehicleMileage
                .Include(vm => vm.Vehicle)
                .FirstOrDefaultAsync(vm => vm.Id == id, cancellationToken);

            if (item is null)
                return ServiceResult.Fail(ErrorNotFound);

            if (item.Vehicle.UserId != userId)
                return ServiceResult.Fail(ErrorForbidden);

            _db.VehicleMileage.Remove(item);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
