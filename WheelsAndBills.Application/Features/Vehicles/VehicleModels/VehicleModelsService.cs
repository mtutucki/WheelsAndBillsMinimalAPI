using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleModels
{
    public class VehicleModelsService : IVehicleModelsService
    {
        private const string ErrorDuplicate = "VehicleModel already exists";
        private const string ErrorBrandNotFound = "VehicleBrand does not exist";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public VehicleModelsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetVehicleModelDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.VehicleModels
                .Select(m => new GetVehicleModelDTO(
                    m.Id,
                    m.BrandId,
                    m.Name
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetVehicleModelDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.VehicleModels
                .Where(m => m.Id == id)
                .Select(m => new GetVehicleModelDTO(
                    m.Id,
                    m.BrandId,
                    m.Name
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetVehicleModelDTO>> CreateAsync(CreateVehicleModelDTO request, CancellationToken cancellationToken = default)
        {
            var brandExists = await _db.VehicleBrands
                .AnyAsync(b => b.Id == request.BrandId, cancellationToken);
            if (!brandExists)
                return ServiceResult<GetVehicleModelDTO>.Fail(ErrorBrandNotFound);

            var exists = await _db.VehicleModels
                .AnyAsync(m => m.BrandId == request.BrandId && m.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<GetVehicleModelDTO>.Fail(ErrorDuplicate);

            var model = new VehicleModel
            {
                Id = Guid.NewGuid(),
                BrandId = request.BrandId,
                Name = request.Name
            };

            _db.VehicleModels.Add(model);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleModelDTO>.Ok(new GetVehicleModelDTO(model.Id, model.BrandId, model.Name));
        }

        public async Task<ServiceResult<GetVehicleModelDTO>> UpdateAsync(Guid id, UpdateVehicleModelDTO request, CancellationToken cancellationToken = default)
        {
            var model = await _db.VehicleModels.FindAsync(new object?[] { id }, cancellationToken);
            if (model is null)
                return ServiceResult<GetVehicleModelDTO>.Fail(ErrorNotFound);

            var brandExists = await _db.VehicleBrands
                .AnyAsync(b => b.Id == request.BrandId, cancellationToken);
            if (!brandExists)
                return ServiceResult<GetVehicleModelDTO>.Fail(ErrorBrandNotFound);

            var exists = await _db.VehicleModels
                .AnyAsync(m =>
                    m.BrandId == request.BrandId &&
                    m.Name == request.Name &&
                    m.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetVehicleModelDTO>.Fail(ErrorDuplicate);

            model.BrandId = request.BrandId;
            model.Name = request.Name;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleModelDTO>.Ok(new GetVehicleModelDTO(model.Id, model.BrandId, model.Name));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var model = await _db.VehicleModels.FindAsync(new object?[] { id }, cancellationToken);
            if (model is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.VehicleModels.Remove(model);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
