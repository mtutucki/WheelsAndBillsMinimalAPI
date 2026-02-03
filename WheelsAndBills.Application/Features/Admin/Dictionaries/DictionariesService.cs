using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;
using DictionaryEntity = WheelsAndBills.Domain.Entities.Admin.Dictionary;

namespace WheelsAndBills.Application.Features.Admin.Dictionaries
{
    public class DictionariesService : IDictionariesService
    {
        private const string ErrorDuplicate = "Dictionary code already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public DictionariesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetDictionaryDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Dictionaries
                .OrderBy(d => d.Code)
                .Select(d => new GetDictionaryDTO(d.Id, d.Code))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetDictionaryDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Dictionaries
                .Where(d => d.Id == id)
                .Select(d => new GetDictionaryDTO(d.Id, d.Code))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetDictionaryDTO>> CreateAsync(CreateDictionaryDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.Dictionaries
                .AnyAsync(d => d.Code == request.Code, cancellationToken);
            if (exists)
                return ServiceResult<GetDictionaryDTO>.Fail(ErrorDuplicate);

            var dictionary = new DictionaryEntity
            {
                Id = Guid.NewGuid(),
                Code = request.Code
            };

            _db.Dictionaries.Add(dictionary);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetDictionaryDTO>.Ok(new GetDictionaryDTO(dictionary.Id, dictionary.Code));
        }

        public async Task<ServiceResult<GetDictionaryDTO>> UpdateAsync(Guid id, UpdateDictionaryDTO request, CancellationToken cancellationToken = default)
        {
            var dictionary = await _db.Dictionaries.FindAsync(new object?[] { id }, cancellationToken);
            if (dictionary is null)
                return ServiceResult<GetDictionaryDTO>.Fail(ErrorNotFound);

            var exists = await _db.Dictionaries
                .AnyAsync(d => d.Code == request.Code && d.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetDictionaryDTO>.Fail(ErrorDuplicate);

            dictionary.Code = request.Code;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetDictionaryDTO>.Ok(new GetDictionaryDTO(dictionary.Id, dictionary.Code));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dictionary = await _db.Dictionaries.FindAsync(new object?[] { id }, cancellationToken);
            if (dictionary is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.Dictionaries.Remove(dictionary);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
