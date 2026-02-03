using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Vehicles;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.VehicleBrands
{
    public class VehicleBrandsService : IVehicleBrandsService
    {
        private const string ErrorDuplicate = "VehicleBrand already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public VehicleBrandsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetVehicleBrandDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.VehicleBrands
                .OrderBy(b => b.Name)
                .Select(b => new GetVehicleBrandDTO(b.Id, b.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetVehicleBrandDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.VehicleBrands
                .Where(b => b.Id == id)
                .Select(b => new GetVehicleBrandDTO(b.Id, b.Name))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetVehicleBrandDTO>> CreateAsync(CreateVehicleBrandDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.VehicleBrands.AnyAsync(b => b.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<GetVehicleBrandDTO>.Fail(ErrorDuplicate);

            var brand = new VehicleBrand
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.VehicleBrands.Add(brand);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleBrandDTO>.Ok(new GetVehicleBrandDTO(brand.Id, brand.Name));
        }

        public async Task<ServiceResult<GetVehicleBrandDTO>> UpdateAsync(Guid id, UpdateVehicleBrandDTO request, CancellationToken cancellationToken = default)
        {
            var brand = await _db.VehicleBrands.FindAsync(new object?[] { id }, cancellationToken);
            if (brand is null)
                return ServiceResult<GetVehicleBrandDTO>.Fail(ErrorNotFound);

            var exists = await _db.VehicleBrands
                .AnyAsync(b => b.Name == request.Name && b.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetVehicleBrandDTO>.Fail(ErrorDuplicate);

            brand.Name = request.Name;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleBrandDTO>.Ok(new GetVehicleBrandDTO(brand.Id, brand.Name));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var brand = await _db.VehicleBrands.FindAsync(new object?[] { id }, cancellationToken);
            if (brand is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.VehicleBrands.Remove(brand);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
