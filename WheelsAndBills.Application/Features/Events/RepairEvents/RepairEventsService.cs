using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Domain.Entities.Events;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.RepairEvents
{
    public class RepairEventsService : IRepairEventsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorVehicleEventMissing = "VehicleEvent does not exist";

        private readonly IAppDbContext _db;

        public RepairEventsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetRepairEventDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.RepairEvents
                .Select(e => new GetRepairEventDTO(
                    e.Id,
                    e.VehicleEventId,
                    e.LaborCost
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetRepairEventDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.RepairEvents
                .Where(e => e.Id == id)
                .Select(e => new GetRepairEventDTO(
                    e.Id,
                    e.VehicleEventId,
                    e.LaborCost
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetRepairEventDTO>> CreateAsync(CreateRepairEventDTO request, CancellationToken cancellationToken = default)
        {
            var vehicleEventExists = await _db.VehicleEvents
                .AnyAsync(v => v.Id == request.VehicleEventId, cancellationToken);
            if (!vehicleEventExists)
                return ServiceResult<GetRepairEventDTO>.Fail(ErrorVehicleEventMissing);

            var ev = new RepairEvent
            {
                Id = Guid.NewGuid(),
                VehicleEventId = request.VehicleEventId,
                LaborCost = request.LaborCost
            };

            _db.RepairEvents.Add(ev);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetRepairEventDTO>.Ok(new GetRepairEventDTO(
                ev.Id,
                ev.VehicleEventId,
                ev.LaborCost
            ));
        }

        public async Task<ServiceResult<GetRepairEventDTO>> UpdateAsync(Guid id, UpdateRepairEventDTO request, CancellationToken cancellationToken = default)
        {
            var ev = await _db.RepairEvents.FindAsync(new object?[] { id }, cancellationToken);
            if (ev is null)
                return ServiceResult<GetRepairEventDTO>.Fail(ErrorNotFound);

            ev.LaborCost = request.LaborCost;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetRepairEventDTO>.Ok(new GetRepairEventDTO(
                ev.Id,
                ev.VehicleEventId,
                ev.LaborCost
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ev = await _db.RepairEvents.FindAsync(new object?[] { id }, cancellationToken);
            if (ev is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.RepairEvents.Remove(ev);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
