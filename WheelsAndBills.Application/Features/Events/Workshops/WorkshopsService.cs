using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Domain.Entities.Events;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.Workshops
{
    public class WorkshopsService : IWorkshopsService
    {
        private const string ErrorDuplicate = "Workshop already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public WorkshopsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetWorkshopDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Workshops
                .OrderBy(w => w.Name)
                .Select(w => new GetWorkshopDTO(w.Id, w.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetWorkshopDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Workshops
                .Where(w => w.Id == id)
                .Select(w => new GetWorkshopDTO(w.Id, w.Name))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetWorkshopDTO>> CreateAsync(CreateWorkshopDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.Workshops.AnyAsync(w => w.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<GetWorkshopDTO>.Fail(ErrorDuplicate);

            var workshop = new Workshop
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.Workshops.Add(workshop);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetWorkshopDTO>.Ok(new GetWorkshopDTO(workshop.Id, workshop.Name));
        }

        public async Task<ServiceResult<GetWorkshopDTO>> UpdateAsync(Guid id, UpdateWorkshopDTO request, CancellationToken cancellationToken = default)
        {
            var workshop = await _db.Workshops.FindAsync(new object?[] { id }, cancellationToken);
            if (workshop is null)
                return ServiceResult<GetWorkshopDTO>.Fail(ErrorNotFound);

            var exists = await _db.Workshops
                .AnyAsync(w => w.Name == request.Name && w.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetWorkshopDTO>.Fail(ErrorDuplicate);

            workshop.Name = request.Name;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetWorkshopDTO>.Ok(new GetWorkshopDTO(workshop.Id, workshop.Name));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var workshop = await _db.Workshops.FindAsync(new object?[] { id }, cancellationToken);
            if (workshop is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.Workshops.Remove(workshop);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
