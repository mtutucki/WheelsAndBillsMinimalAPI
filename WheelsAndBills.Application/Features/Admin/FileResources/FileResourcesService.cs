using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;
using FileResourceEntity = WheelsAndBills.Domain.Entities.Admin.FileResource;

namespace WheelsAndBills.Application.Features.Admin.FileResources
{
    public class FileResourcesService : IFileResourcesService
    {
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public FileResourcesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetFileResourceDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.FileResources
                .OrderByDescending(f => f.UploadedAt)
                .Select(f => new GetFileResourceDTO(
                    f.Id,
                    f.FileName,
                    f.FilePath,
                    f.UploadedAt
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetFileResourceDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.FileResources
                .Where(f => f.Id == id)
                .Select(f => new GetFileResourceDTO(
                    f.Id,
                    f.FileName,
                    f.FilePath,
                    f.UploadedAt
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetFileResourceDTO>> CreateAsync(CreateFileResourceDTO request, CancellationToken cancellationToken = default)
        {
            var file = new FileResourceEntity
            {
                Id = Guid.NewGuid(),
                FileName = request.FileName,
                FilePath = request.FilePath,
                UploadedAt = DateTime.UtcNow
            };

            _db.FileResources.Add(file);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetFileResourceDTO>.Ok(new GetFileResourceDTO(
                file.Id,
                file.FileName,
                file.FilePath,
                file.UploadedAt
            ));
        }

        public async Task<ServiceResult<GetFileResourceDTO>> UpdateAsync(Guid id, UpdateFileResourceDTO request, CancellationToken cancellationToken = default)
        {
            var file = await _db.FileResources.FindAsync(new object?[] { id }, cancellationToken);
            if (file is null)
                return ServiceResult<GetFileResourceDTO>.Fail(ErrorNotFound);

            file.FileName = request.FileName;
            file.FilePath = request.FilePath;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetFileResourceDTO>.Ok(new GetFileResourceDTO(
                file.Id,
                file.FileName,
                file.FilePath,
                file.UploadedAt
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var file = await _db.FileResources.FindAsync(new object?[] { id }, cancellationToken);
            if (file is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.FileResources.Remove(file);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
