using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Domain.Entities.Events;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.EventTypes
{
    public class EventTypesService : IEventTypesService
    {
        private const string ErrorDuplicate = "Event type already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public EventTypesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetEventTypeDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.EventTypes
                .OrderBy(t => t.Name)
                .Select(t => new GetEventTypeDTO(t.Id, t.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetEventTypeDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.EventTypes
                .Where(t => t.Id == id)
                .Select(t => new GetEventTypeDTO(t.Id, t.Name))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetEventTypeDTO>> CreateAsync(CreateEventTypeDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.EventTypes
                .AnyAsync(t => t.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<GetEventTypeDTO>.Fail(ErrorDuplicate);

            var type = new EventType
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.EventTypes.Add(type);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetEventTypeDTO>.Ok(new GetEventTypeDTO(type.Id, type.Name));
        }

        public async Task<ServiceResult<GetEventTypeDTO>> UpdateAsync(Guid id, UpdateEventTypeDTO request, CancellationToken cancellationToken = default)
        {
            var type = await _db.EventTypes.FindAsync(new object?[] { id }, cancellationToken);
            if (type is null)
                return ServiceResult<GetEventTypeDTO>.Fail(ErrorNotFound);

            var exists = await _db.EventTypes
                .AnyAsync(t => t.Name == request.Name && t.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetEventTypeDTO>.Fail(ErrorDuplicate);

            type.Name = request.Name;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetEventTypeDTO>.Ok(new GetEventTypeDTO(type.Id, type.Name));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var type = await _db.EventTypes.FindAsync(new object?[] { id }, cancellationToken);
            if (type is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.EventTypes.Remove(type);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
