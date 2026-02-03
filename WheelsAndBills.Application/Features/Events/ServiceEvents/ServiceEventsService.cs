using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Domain.Entities.Events;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.ServiceEvents
{
    public class ServiceEventsService : IServiceEventsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorVehicleEventMissing = "VehicleEvent does not exist";
        private const string ErrorWorkshopMissing = "Workshop does not exist";

        private readonly IAppDbContext _db;

        public ServiceEventsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetServiceEventDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.ServiceEvents
                .Select(e => new GetServiceEventDTO(
                    e.Id,
                    e.VehicleEventId,
                    e.WorkshopId
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetServiceEventDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.ServiceEvents
                .Where(e => e.Id == id)
                .Select(e => new GetServiceEventDTO(
                    e.Id,
                    e.VehicleEventId,
                    e.WorkshopId
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetServiceEventDTO>> CreateAsync(CreateServiceEventDTO request, CancellationToken cancellationToken = default)
        {
            var vehicleEventExists = await _db.VehicleEvents
                .AnyAsync(v => v.Id == request.VehicleEventId, cancellationToken);
            if (!vehicleEventExists)
                return ServiceResult<GetServiceEventDTO>.Fail(ErrorVehicleEventMissing);

            var workshopExists = await _db.Workshops
                .AnyAsync(w => w.Id == request.WorkshopId, cancellationToken);
            if (!workshopExists)
                return ServiceResult<GetServiceEventDTO>.Fail(ErrorWorkshopMissing);

            var ev = new ServiceEvent
            {
                Id = Guid.NewGuid(),
                VehicleEventId = request.VehicleEventId,
                WorkshopId = request.WorkshopId
            };

            _db.ServiceEvents.Add(ev);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetServiceEventDTO>.Ok(new GetServiceEventDTO(
                ev.Id,
                ev.VehicleEventId,
                ev.WorkshopId
            ));
        }

        public async Task<ServiceResult<GetServiceEventDTO>> UpdateAsync(Guid id, UpdateServiceEventDTO request, CancellationToken cancellationToken = default)
        {
            var ev = await _db.ServiceEvents.FindAsync(new object?[] { id }, cancellationToken);
            if (ev is null)
                return ServiceResult<GetServiceEventDTO>.Fail(ErrorNotFound);

            var workshopExists = await _db.Workshops
                .AnyAsync(w => w.Id == request.WorkshopId, cancellationToken);
            if (!workshopExists)
                return ServiceResult<GetServiceEventDTO>.Fail(ErrorWorkshopMissing);

            ev.WorkshopId = request.WorkshopId;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetServiceEventDTO>.Ok(new GetServiceEventDTO(
                ev.Id,
                ev.VehicleEventId,
                ev.WorkshopId
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ev = await _db.ServiceEvents.FindAsync(new object?[] { id }, cancellationToken);
            if (ev is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.ServiceEvents.Remove(ev);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
