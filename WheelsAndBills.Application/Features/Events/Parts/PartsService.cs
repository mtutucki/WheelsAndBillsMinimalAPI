using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Domain.Entities.Events;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.Parts
{
    public class PartsService : IPartsService
    {
        private const string ErrorDuplicate = "Part already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public PartsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetPartDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Parts
                .OrderBy(p => p.Name)
                .Select(p => new GetPartDTO(p.Id, p.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetPartDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Parts
                .Where(p => p.Id == id)
                .Select(p => new GetPartDTO(p.Id, p.Name))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetPartDTO>> CreateAsync(CreatePartDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.Parts.AnyAsync(p => p.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<GetPartDTO>.Fail(ErrorDuplicate);

            var part = new Part
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.Parts.Add(part);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetPartDTO>.Ok(new GetPartDTO(part.Id, part.Name));
        }

        public async Task<ServiceResult<GetPartDTO>> UpdateAsync(Guid id, UpdatePartDTO request, CancellationToken cancellationToken = default)
        {
            var part = await _db.Parts.FindAsync(new object?[] { id }, cancellationToken);
            if (part is null)
                return ServiceResult<GetPartDTO>.Fail(ErrorNotFound);

            var exists = await _db.Parts
                .AnyAsync(p => p.Name == request.Name && p.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetPartDTO>.Fail(ErrorDuplicate);

            part.Name = request.Name;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetPartDTO>.Ok(new GetPartDTO(part.Id, part.Name));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var part = await _db.Parts.FindAsync(new object?[] { id }, cancellationToken);
            if (part is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.Parts.Remove(part);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
