using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleStatuses
{
    public class VehicleStatusesService : IVehicleStatusesService
    {
        private static readonly Guid DeletedStatusId = Guid.Parse("85C30BAB-7FA3-4124-BE5D-1E220CACE01F");
        private const string ErrorDuplicate = "VehicleStatus already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public VehicleStatusesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetVehicleStatusDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.VehicleStatuses
                .Where(s => s.Id != DeletedStatusId)
                .OrderBy(s => s.Name)
                .Select(s => new GetVehicleStatusDTO(s.Id, s.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetVehicleStatusDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.VehicleStatuses
                .Where(s => s.Id == id)
                .Select(s => new GetVehicleStatusDTO(s.Id, s.Name))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetVehicleStatusDTO>> CreateAsync(CreateVehicleStatusDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.VehicleStatuses
                .AnyAsync(s => s.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<GetVehicleStatusDTO>.Fail(ErrorDuplicate);

            var status = new VehicleStatus
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.VehicleStatuses.Add(status);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleStatusDTO>.Ok(new GetVehicleStatusDTO(status.Id, status.Name));
        }

        public async Task<ServiceResult<GetVehicleStatusDTO>> UpdateAsync(Guid id, UpdateVehicleStatusDTO request, CancellationToken cancellationToken = default)
        {
            var status = await _db.VehicleStatuses.FindAsync(new object?[] { id }, cancellationToken);
            if (status is null)
                return ServiceResult<GetVehicleStatusDTO>.Fail(ErrorNotFound);

            var exists = await _db.VehicleStatuses
                .AnyAsync(s => s.Name == request.Name && s.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetVehicleStatusDTO>.Fail(ErrorDuplicate);

            status.Name = request.Name;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleStatusDTO>.Ok(new GetVehicleStatusDTO(status.Id, status.Name));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var status = await _db.VehicleStatuses.FindAsync(new object?[] { id }, cancellationToken);
            if (status is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.VehicleStatuses.Remove(status);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
