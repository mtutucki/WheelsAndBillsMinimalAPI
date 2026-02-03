using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using EventPartEntity = WheelsAndBills.Domain.Entities.Events.EventPart;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.EventParts
{
    public class EventPartsService : IEventPartsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorRepairEventMissing = "RepairEvent does not exist";
        private const string ErrorPartMissing = "Part does not exist";
        private const string ErrorDuplicate = "Part already added to repair event";
        private const string ErrorIdMismatch = "RepairEventId mismatch";

        private readonly IAppDbContext _db;

        public EventPartsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetEventPartDTO>> GetByRepairEventAsync(Guid repairEventId, CancellationToken cancellationToken = default)
        {
            return await _db.EventParts
                .Where(ep => ep.RepairEventId == repairEventId)
                .Select(ep => new GetEventPartDTO(
                    ep.RepairEventId,
                    ep.PartId,
                    ep.Price
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetEventPartDTO>> CreateAsync(Guid repairEventId, CreateEventPartDTO request, CancellationToken cancellationToken = default)
        {
            if (repairEventId != request.RepairEventId)
                return ServiceResult<GetEventPartDTO>.Fail(ErrorIdMismatch);

            var repairEventExists = await _db.RepairEvents
                .AnyAsync(e => e.Id == request.RepairEventId, cancellationToken);
            if (!repairEventExists)
                return ServiceResult<GetEventPartDTO>.Fail(ErrorRepairEventMissing);

            var partExists = await _db.Parts
                .AnyAsync(p => p.Id == request.PartId, cancellationToken);
            if (!partExists)
                return ServiceResult<GetEventPartDTO>.Fail(ErrorPartMissing);

            var exists = await _db.EventParts
                .AnyAsync(ep =>
                    ep.RepairEventId == request.RepairEventId &&
                    ep.PartId == request.PartId, cancellationToken);
            if (exists)
                return ServiceResult<GetEventPartDTO>.Fail(ErrorDuplicate);

            var eventPart = new EventPartEntity
            {
                RepairEventId = request.RepairEventId,
                PartId = request.PartId,
                Price = request.Price
            };

            _db.EventParts.Add(eventPart);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetEventPartDTO>.Ok(new GetEventPartDTO(
                eventPart.RepairEventId,
                eventPart.PartId,
                eventPart.Price
            ));
        }

        public async Task<ServiceResult<GetEventPartDTO>> UpdateAsync(Guid repairEventId, Guid partId, UpdateEventPartDTO request, CancellationToken cancellationToken = default)
        {
            var eventPart = await _db.EventParts
                .FirstOrDefaultAsync(ep =>
                    ep.RepairEventId == repairEventId &&
                    ep.PartId == partId, cancellationToken);

            if (eventPart is null)
                return ServiceResult<GetEventPartDTO>.Fail(ErrorNotFound);

            eventPart.Price = request.Price;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetEventPartDTO>.Ok(new GetEventPartDTO(
                eventPart.RepairEventId,
                eventPart.PartId,
                eventPart.Price
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid repairEventId, Guid partId, CancellationToken cancellationToken = default)
        {
            var eventPart = await _db.EventParts
                .FirstOrDefaultAsync(ep =>
                    ep.RepairEventId == repairEventId &&
                    ep.PartId == partId, cancellationToken);

            if (eventPart is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.EventParts.Remove(eventPart);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
