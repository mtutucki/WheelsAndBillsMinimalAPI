using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Domain.Entities.Events;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.FuelingEvents
{
    public class FuelingEventsService : IFuelingEventsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorVehicleEventMissing = "VehicleEvent does not exist";

        private readonly IAppDbContext _db;

        public FuelingEventsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetFuelingEventDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.FuelingEvents
                .Select(e => new GetFuelingEventDTO(
                    e.Id,
                    e.VehicleEventId,
                    e.Liters,
                    e.TotalPrice
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetFuelingEventDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.FuelingEvents
                .Where(e => e.Id == id)
                .Select(e => new GetFuelingEventDTO(
                    e.Id,
                    e.VehicleEventId,
                    e.Liters,
                    e.TotalPrice
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetFuelingEventDTO>> CreateAsync(CreateFuelingEventDTO request, CancellationToken cancellationToken = default)
        {
            var vehicleEventExists = await _db.VehicleEvents
                .AnyAsync(v => v.Id == request.VehicleEventId, cancellationToken);
            if (!vehicleEventExists)
                return ServiceResult<GetFuelingEventDTO>.Fail(ErrorVehicleEventMissing);

            var ev = new FuelingEvent
            {
                Id = Guid.NewGuid(),
                VehicleEventId = request.VehicleEventId,
                Liters = request.Liters,
                TotalPrice = request.TotalPrice
            };

            _db.FuelingEvents.Add(ev);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetFuelingEventDTO>.Ok(new GetFuelingEventDTO(
                ev.Id,
                ev.VehicleEventId,
                ev.Liters,
                ev.TotalPrice
            ));
        }

        public async Task<ServiceResult<GetFuelingEventDTO>> UpdateAsync(Guid id, UpdateFuelingEventDTO request, CancellationToken cancellationToken = default)
        {
            var ev = await _db.FuelingEvents.FindAsync(new object?[] { id }, cancellationToken);
            if (ev is null)
                return ServiceResult<GetFuelingEventDTO>.Fail(ErrorNotFound);

            ev.Liters = request.Liters;
            ev.TotalPrice = request.TotalPrice;

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetFuelingEventDTO>.Ok(new GetFuelingEventDTO(
                ev.Id,
                ev.VehicleEventId,
                ev.Liters,
                ev.TotalPrice
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ev = await _db.FuelingEvents.FindAsync(new object?[] { id }, cancellationToken);
            if (ev is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.FuelingEvents.Remove(ev);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
