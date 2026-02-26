using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Events;
using WheelsAndBills.Application.Helpers.Events;
using VehicleEventEntity = WheelsAndBills.Domain.Entities.Events.VehicleEvent;
using VehicleMileageEntity = WheelsAndBills.Domain.Entities.Vehicles.VehicleMileage;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.VehicleEvents
{
    public class VehicleEventsService : IVehicleEventsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorForbidden = "Forbidden";
        private const string ErrorVehicleMissing = "Vehicle does not exist";
        private const string ErrorVehicleNotOwned = "Vehicle does not belong to user";
        private const string ErrorEventTypeMissing = "EventType does not exist";
        private const string ErrorMileageTooLow = "Mileage cannot be lower than last recorded mileage";

        private readonly IAppDbContext _db;

        public VehicleEventsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetVehicleEventDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.VehicleEvents
                .Select(e => new GetVehicleEventDTO(
                    e.Id,
                    e.VehicleId,
                    e.EventTypeId,
                    e.EventDate,
                    e.Mileage,
                    e.Description
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetVehicleEventDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.VehicleEvents
                .Where(e => e.Id == id)
                .Select(e => new GetVehicleEventDTO(
                    e.Id,
                    e.VehicleId,
                    e.EventTypeId,
                    e.EventDate,
                    e.Mileage,
                    e.Description
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetVehicleEventDTO>> CreateAsync(CreateVehicleEventDTO request, CancellationToken cancellationToken = default)
        {
            var vehicleExists = await _db.Vehicles
                .AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
            if (!vehicleExists)
                return ServiceResult<GetVehicleEventDTO>.Fail(ErrorVehicleMissing);

            var eventTypeExists = await _db.EventTypes
                .AnyAsync(et => et.Id == request.EventTypeId, cancellationToken);
            if (!eventTypeExists)
                return ServiceResult<GetVehicleEventDTO>.Fail(ErrorEventTypeMissing);

            var ev = new VehicleEventEntity
            {
                Id = Guid.NewGuid(),
                VehicleId = request.VehicleId,
                EventTypeId = request.EventTypeId,
                EventDate = request.EventDate,
                Mileage = request.Mileage,
                Description = request.Description
            };

            _db.VehicleEvents.Add(ev);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleEventDTO>.Ok(new GetVehicleEventDTO(
                ev.Id,
                ev.VehicleId,
                ev.EventTypeId,
                ev.EventDate,
                ev.Mileage,
                ev.Description
            ));
        }

        public async Task<ServiceResult<GetVehicleEventDTO>> UpdateAsync(Guid id, UpdateVehicleEventDTO request, CancellationToken cancellationToken = default)
        {
            var ev = await _db.VehicleEvents.FindAsync(new object?[] { id }, cancellationToken);
            if (ev is null)
                return ServiceResult<GetVehicleEventDTO>.Fail(ErrorNotFound);

            var eventTypeExists = await _db.EventTypes
                .AnyAsync(et => et.Id == request.EventTypeId, cancellationToken);
            if (!eventTypeExists)
                return ServiceResult<GetVehicleEventDTO>.Fail(ErrorEventTypeMissing);

            ev.EventTypeId = request.EventTypeId;
            ev.EventDate = request.EventDate;
            ev.Mileage = request.Mileage;
            ev.Description = request.Description;

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleEventDTO>.Ok(new GetVehicleEventDTO(
                ev.Id,
                ev.VehicleId,
                ev.EventTypeId,
                ev.EventDate,
                ev.Mileage,
                ev.Description
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ev = await _db.VehicleEvents.FindAsync(new object?[] { id }, cancellationToken);
            if (ev is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.VehicleEvents.Remove(ev);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<GetVehicleEventDTO>> UpdateForUserAsync(Guid userId, Guid id, UpdateMyVehicleEventDTO request, CancellationToken cancellationToken = default)
        {
            var ev = await _db.VehicleEvents
                .Include(e => e.Vehicle)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (ev is null)
                return ServiceResult<GetVehicleEventDTO>.Fail(ErrorNotFound);

            if (ev.Vehicle.UserId != userId)
                return ServiceResult<GetVehicleEventDTO>.Fail(ErrorForbidden);

            var eventTypeExists = await _db.EventTypes
                .AnyAsync(et => et.Id == request.EventTypeId, cancellationToken);
            if (!eventTypeExists)
                return ServiceResult<GetVehicleEventDTO>.Fail(ErrorEventTypeMissing);

            ev.EventTypeId = request.EventTypeId;
            ev.EventDate = request.EventDate.Date;
            ev.Mileage = request.Mileage;
            ev.Description = request.Description;

            var fueling = await _db.FuelingEvents
                .Where(f => f.VehicleEventId == ev.Id)
                .ToListAsync(cancellationToken);
            _db.FuelingEvents.RemoveRange(fueling);

            var repairEvents = await _db.RepairEvents
                .Where(r => r.VehicleEventId == ev.Id)
                .ToListAsync(cancellationToken);
            var repairIds = repairEvents.Select(r => r.Id).ToList();
            if (repairIds.Count > 0)
            {
                var parts = await _db.EventParts
                    .Where(p => repairIds.Contains(p.RepairEventId))
                    .ToListAsync(cancellationToken);
                _db.EventParts.RemoveRange(parts);
            }
            _db.RepairEvents.RemoveRange(repairEvents);

            var services = await _db.ServiceEvents
                .Where(s => s.VehicleEventId == ev.Id)
                .ToListAsync(cancellationToken);
            _db.ServiceEvents.RemoveRange(services);

            var costs = await _db.Costs
                .Where(c => c.VehicleEventId == ev.Id)
                .ToListAsync(cancellationToken);
            _db.Costs.RemoveRange(costs);

            var createLike = new CreateMyVehicleEventDTO(
                ev.VehicleId,
                request.EventTypeId,
                request.EventDate,
                request.Mileage,
                request.Description,
                request.Data
            );

            if (request.Data is not null && request.Data.Count > 0)
            {
                await VehicleEventDetailsHandler.Handle(createLike, ev.Id, _db);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetVehicleEventDTO>.Ok(new GetVehicleEventDTO(
                ev.Id,
                ev.VehicleId,
                ev.EventTypeId,
                ev.EventDate,
                ev.Mileage,
                ev.Description
            ));
        }

        public async Task<GetMyVehicleEventDetailsDTO?> GetDetailsForUserAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        {
            var ev = await _db.VehicleEvents
                .Include(e => e.Vehicle)
                .FirstOrDefaultAsync(e => e.Id == id && e.Vehicle.UserId == userId, cancellationToken);

            if (ev is null)
                return null;

            Dictionary<string, object?>? data = null;

            var fueling = await _db.FuelingEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.VehicleEventId == ev.Id, cancellationToken);
            if (fueling is not null)
            {
                data ??= new Dictionary<string, object?>();
                data["liters"] = fueling.Liters;
                data["totalCost"] = fueling.TotalPrice;
            }

            var service = await _db.ServiceEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.VehicleEventId == ev.Id, cancellationToken);
            if (service is not null)
            {
                data ??= new Dictionary<string, object?>();
                data["workshopId"] = service.WorkshopId;
            }

            var repair = await _db.RepairEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.VehicleEventId == ev.Id, cancellationToken);
            if (repair is not null)
            {
                data ??= new Dictionary<string, object?>();
                data["laborCost"] = repair.LaborCost;

                var parts = await _db.EventParts
                    .Where(p => p.RepairEventId == repair.Id)
                    .Select(p => new { partId = p.PartId, price = p.Price })
                    .ToListAsync(cancellationToken);

                if (parts.Count > 0)
                    data["parts"] = parts;
            }

            // For status-change events, we don't store historical status;
            data ??= new Dictionary<string, object?>();
            data["vehicleStatusId"] = ev.Vehicle.StatusId;

            return new GetMyVehicleEventDetailsDTO(
                ev.Id,
                ev.VehicleId,
                ev.EventTypeId,
                ev.EventDate,
                ev.Mileage,
                ev.Description,
                data
            );
        }

        public async Task<ServiceResult> DeleteForUserAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _db.VehicleEvents
                .Include(vm => vm.Vehicle)
                .FirstOrDefaultAsync(vm => vm.Id == id, cancellationToken);

            if (item is null)
                return ServiceResult.Fail(ErrorNotFound);

            if (item.Vehicle.UserId != userId)
                return ServiceResult.Fail(ErrorForbidden);

            _db.VehicleEvents.Remove(item);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult<GetVehicleEventDTO>> CreateForUserAsync(Guid userId, CreateMyVehicleEventDTO request, CancellationToken cancellationToken = default)
        {
            var vehicle = await _db.Vehicles
                .FirstOrDefaultAsync(v => v.Id == request.VehicleId && v.UserId == userId, cancellationToken);

            if (vehicle is null)
                return ServiceResult<GetVehicleEventDTO>.Fail(ErrorVehicleNotOwned);

            var eventTypeExists = await _db.EventTypes
                .AnyAsync(et => et.Id == request.EventTypeId, cancellationToken);

            if (!eventTypeExists)
                return ServiceResult<GetVehicleEventDTO>.Fail(ErrorEventTypeMissing);

            int lastMileage = 0;
            if (request.Mileage > 0)
            {
                lastMileage = await _db.VehicleMileage
                    .Where(m => m.VehicleId == request.VehicleId)
                    .OrderByDescending(m => m.Mileage)
                    .Select(m => m.Mileage)
                    .FirstOrDefaultAsync(cancellationToken);

                if (request.Mileage < lastMileage)
                {
                    return ServiceResult<GetVehicleEventDTO>.Fail(
                        $"{ErrorMileageTooLow} ({lastMileage})"
                    );
                }
            }

            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            var ev = new VehicleEventEntity
            {
                Id = Guid.NewGuid(),
                VehicleId = request.VehicleId,
                EventTypeId = request.EventTypeId,
                EventDate = request.EventDate,
                Mileage = request.Mileage,
                Description = request.Description,
                CreatedAt = DateTime.Now
            };

            if (request.Mileage > 0 && request.Mileage > lastMileage)
            {
                _db.VehicleMileage.Add(new VehicleMileageEntity
                {
                    Id = Guid.NewGuid(),
                    VehicleId = request.VehicleId,
                    Mileage = request.Mileage,
                    Date = request.EventDate.Date
                });
            }

            _db.VehicleEvents.Add(ev);
            await _db.SaveChangesAsync(cancellationToken);

            await VehicleEventDetailsHandler.Handle(request, ev.Id, _db);
            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return ServiceResult<GetVehicleEventDTO>.Ok(new GetVehicleEventDTO(
                ev.Id,
                ev.VehicleId,
                ev.EventTypeId,
                ev.EventDate,
                ev.Mileage,
                ev.Description
            ));
        }
    }
}
