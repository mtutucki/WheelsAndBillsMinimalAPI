using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleTypes
{
    public class VehicleTypesService : IVehicleTypesService
    {
        private const string ErrorDuplicate = "VehicleType already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public VehicleTypesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetVehicleTypeDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.VehicleTypes
                .OrderBy(t => t.Name)
                .Select(t => new GetVehicleTypeDTO(t.Id, t.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetVehicleTypeDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.VehicleTypes
                .Where(t => t.Id == id)
                .Select(t => new GetVehicleTypeDTO(t.Id, t.Name))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetVehicleTypeDTO>> CreateAsync(CreateVehicleTypeDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.VehicleTypes.AnyAsync(t => t.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<GetVehicleTypeDTO>.Fail(ErrorDuplicate);

            var type = new VehicleType
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.VehicleTypes.Add(type);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleTypeDTO>.Ok(new GetVehicleTypeDTO(type.Id, type.Name));
        }

        public async Task<ServiceResult<GetVehicleTypeDTO>> UpdateAsync(Guid id, UpdateVehicleTypeDTO request, CancellationToken cancellationToken = default)
        {
            var type = await _db.VehicleTypes.FindAsync(new object?[] { id }, cancellationToken);
            if (type is null)
                return ServiceResult<GetVehicleTypeDTO>.Fail(ErrorNotFound);

            var exists = await _db.VehicleTypes
                .AnyAsync(t => t.Name == request.Name && t.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetVehicleTypeDTO>.Fail(ErrorDuplicate);

            type.Name = request.Name;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleTypeDTO>.Ok(new GetVehicleTypeDTO(type.Id, type.Name));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var type = await _db.VehicleTypes.FindAsync(new object?[] { id }, cancellationToken);
            if (type is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.VehicleTypes.Remove(type);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
