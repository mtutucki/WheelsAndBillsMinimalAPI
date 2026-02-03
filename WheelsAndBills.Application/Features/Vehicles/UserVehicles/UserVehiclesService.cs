using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.DTOs.Vehicles;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace WheelsAndBills.Application.Features.Vehicles.UserVehicles
{
    public class UserVehiclesService : IUserVehiclesService
    {
        private static readonly Guid DeletedStatusId = Guid.Parse("85C30BAB-7FA3-4124-BE5D-1E220CACE01F");
        private readonly IAppDbContext _db;

        public UserVehiclesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetVehiclesByUserDTO>> GetUserVehiclesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.Vehicles
                .Where(v => v.UserId == userId && v.StatusId != DeletedStatusId)
                .OrderByDescending(v => v.Status.Name == "Aktywny")
                .ThenBy(v => v.Brand.Name)
                .Include(v => v.Brand)
                .Include(v => v.Model)
                .Include(v => v.Type)
                .Include(v => v.Status)
                .Select(v => new GetVehiclesByUserDTO(
                    v.Id,
                    v.Vin,
                    v.Year,
                    new LookupDTO(v.BrandId, v.Brand.Name),
                    new LookupDTO(v.ModelId, v.Model.Name),
                    new LookupDTO(v.TypeId, v.Type.Name),
                    new LookupDTO(v.StatusId, v.Status.Name)
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<VehicleDetailsDTO?> GetUserVehicleByIdAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default)
        {
            return await _db.Vehicles
                .AsNoTracking()
                .Where(v => v.Id == vehicleId && v.UserId == userId)
                .Include(v => v.Brand)
                .Include(v => v.Model)
                .Include(v => v.Type)
                .Include(v => v.Status)
                .Select(v => new VehicleDetailsDTO(
                    v.Id,
                    v.Vin,
                    v.Year,
                    new LookupDTO(v.BrandId, v.Brand.Name),
                    new LookupDTO(v.ModelId, v.Model.Name),
                    new LookupDTO(v.TypeId, v.Type.Name),
                    new LookupDTO(v.StatusId, v.Status.Name),
                    _db.VehicleMileage
                        .Where(m => m.VehicleId == v.Id)
                        .OrderByDescending(m => m.Date)
                        .Select(m => new VehicleMileageDTO(
                            m.Id,
                            m.Mileage,
                            m.Date
                        ))
                        .ToList(),
                    _db.VehicleEvents
                        .Where(e => e.VehicleId == v.Id)
                        .Include(e => e.EventType)
                        .OrderByDescending(e => e.EventDate)
                        .ThenByDescending(e => e.Mileage)
                        .ThenByDescending(e => e.CreatedAt)
                        .Select(e => new VehicleEventDTO(
                            e.Id,
                            new LookupDTO(e.EventTypeId, e.EventType.Name),
                            e.EventDate,
                            e.Mileage,
                            e.Description
                        ))
                        .ToList(),
                    _db.VehicleNotes
                        .Where(n => n.VehicleId == v.Id)
                        .OrderByDescending(n => n.CreatedAt)
                        .Select(n => new VehicleNoteDTO(
                            n.Id,
                            n.Content,
                            n.CreatedAt,
                            n.UserId
                        ))
                        .ToList()
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Guid> CreateVehicleAsync(Guid userId, CreateVehicleRequestDTO request, CancellationToken cancellationToken = default)
        {
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Vin = request.Vin,
                Year = request.Year,
                BrandId = request.BrandId,
                ModelId = request.ModelId,
                TypeId = request.TypeId,
                StatusId = request.StatusId
            };

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync(cancellationToken);

            return vehicle.Id;
        }

        public async Task<bool> DeleteVehicleAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default)
        {
            var vehicle = await _db.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId, cancellationToken);

            if (vehicle is null)
                return false;

            vehicle.StatusId = DeletedStatusId;
            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
