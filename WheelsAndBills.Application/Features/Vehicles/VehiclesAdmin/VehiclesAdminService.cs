using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehiclesAdmin
{
    public class VehiclesAdminService : IVehiclesAdminService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorUserMissing = "User does not exist";
        private const string ErrorVinDuplicate = "Vehicle with this VIN already exists";

        private readonly IAppDbContext _db;

        public VehiclesAdminService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetVehicleDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Vehicles
                .Select(v => new GetVehicleDTO(
                    v.Id,
                    v.UserId,
                    v.Vin,
                    v.Year,
                    v.BrandId,
                    v.ModelId,
                    v.TypeId,
                    v.StatusId
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetVehicleDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Vehicles
                .Where(v => v.Id == id)
                .Select(v => new GetVehicleDTO(
                    v.Id,
                    v.UserId,
                    v.Vin,
                    v.Year,
                    v.BrandId,
                    v.ModelId,
                    v.TypeId,
                    v.StatusId
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetVehicleDTO>> CreateAsync(CreateVehicleDTO request, CancellationToken cancellationToken = default)
        {
            var userExists = await _db.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
            if (!userExists)
                return ServiceResult<GetVehicleDTO>.Fail(ErrorUserMissing);

            var vinExists = await _db.Vehicles.AnyAsync(v => v.Vin == request.Vin, cancellationToken);
            if (vinExists)
                return ServiceResult<GetVehicleDTO>.Fail(ErrorVinDuplicate);

            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Vin = request.Vin,
                Year = request.Year,
                BrandId = request.BrandId,
                ModelId = request.ModelId,
                TypeId = request.TypeId,
                StatusId = request.StatusId
            };

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleDTO>.Ok(new GetVehicleDTO(
                vehicle.Id,
                vehicle.UserId,
                vehicle.Vin,
                vehicle.Year,
                vehicle.BrandId,
                vehicle.ModelId,
                vehicle.TypeId,
                vehicle.StatusId
            ));
        }

        public async Task<ServiceResult<GetVehicleDTO>> UpdateAsync(Guid id, UpdateVehicleDTO request, CancellationToken cancellationToken = default)
        {
            var vehicle = await _db.Vehicles.FindAsync(new object?[] { id }, cancellationToken);
            if (vehicle is null)
                return ServiceResult<GetVehicleDTO>.Fail(ErrorNotFound);

            var vinExists = await _db.Vehicles
                .AnyAsync(v => v.Vin == request.Vin && v.Id != id, cancellationToken);
            if (vinExists)
                return ServiceResult<GetVehicleDTO>.Fail(ErrorVinDuplicate);

            vehicle.Vin = request.Vin;
            vehicle.Year = request.Year;
            vehicle.BrandId = request.BrandId;
            vehicle.ModelId = request.ModelId;
            vehicle.TypeId = request.TypeId;
            vehicle.StatusId = request.StatusId;

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleDTO>.Ok(new GetVehicleDTO(
                vehicle.Id,
                vehicle.UserId,
                vehicle.Vin,
                vehicle.Year,
                vehicle.BrandId,
                vehicle.ModelId,
                vehicle.TypeId,
                vehicle.StatusId
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var vehicle = await _db.Vehicles.FindAsync(new object?[] { id }, cancellationToken);
            if (vehicle is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.Vehicles.Remove(vehicle);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
