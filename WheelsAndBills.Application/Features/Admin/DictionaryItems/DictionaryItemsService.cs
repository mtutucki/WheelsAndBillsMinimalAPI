using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;
using DictionaryItemEntity = WheelsAndBills.Domain.Entities.Admin.DictionaryItem;

namespace WheelsAndBills.Application.Features.Admin.DictionaryItems
{
    public class DictionaryItemsService : IDictionaryItemsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorDictionaryMissing = "Dictionary does not exist";
        private const string ErrorDuplicate = "Dictionary item already exists";

        private readonly IAppDbContext _db;

        public DictionaryItemsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetDictionaryItemDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.DictionaryItems
                .OrderBy(i => i.Value)
                .Select(i => new GetDictionaryItemDTO(i.Id, i.DictionaryId, i.Value))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<GetDictionaryItemDTO>> GetByDictionaryIdAsync(Guid dictionaryId, CancellationToken cancellationToken = default)
        {
            return await _db.DictionaryItems
                .Where(i => i.DictionaryId == dictionaryId)
                .OrderBy(i => i.Value)
                .Select(i => new GetDictionaryItemDTO(i.Id, i.DictionaryId, i.Value))
                .ToListAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetDictionaryItemDTO>> CreateAsync(CreateDictionaryItemDTO request, CancellationToken cancellationToken = default)
        {
            var dictionaryExists = await _db.Dictionaries
                .AnyAsync(d => d.Id == request.DictionaryId, cancellationToken);
            if (!dictionaryExists)
                return ServiceResult<GetDictionaryItemDTO>.Fail(ErrorDictionaryMissing);

            var exists = await _db.DictionaryItems
                .AnyAsync(i => i.DictionaryId == request.DictionaryId && i.Value == request.Value, cancellationToken);
            if (exists)
                return ServiceResult<GetDictionaryItemDTO>.Fail(ErrorDuplicate);

            var item = new DictionaryItemEntity
            {
                Id = Guid.NewGuid(),
                DictionaryId = request.DictionaryId,
                Value = request.Value
            };

            _db.DictionaryItems.Add(item);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetDictionaryItemDTO>.Ok(new GetDictionaryItemDTO(item.Id, item.DictionaryId, item.Value));
        }

        public async Task<ServiceResult<GetDictionaryItemDTO>> UpdateAsync(Guid id, UpdateDictionaryItemDTO request, CancellationToken cancellationToken = default)
        {
            var item = await _db.DictionaryItems.FindAsync(new object?[] { id }, cancellationToken);
            if (item is null)
                return ServiceResult<GetDictionaryItemDTO>.Fail(ErrorNotFound);

            var exists = await _db.DictionaryItems
                .AnyAsync(i => i.DictionaryId == item.DictionaryId && i.Value == request.Value && i.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetDictionaryItemDTO>.Fail(ErrorDuplicate);

            item.Value = request.Value;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetDictionaryItemDTO>.Ok(new GetDictionaryItemDTO(item.Id, item.DictionaryId, item.Value));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _db.DictionaryItems.FindAsync(new object?[] { id }, cancellationToken);
            if (item is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.DictionaryItems.Remove(item);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
