using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;
using ContentPageEntity = WheelsAndBills.Domain.Entities.Admin.ContentPage;

namespace WheelsAndBills.Application.Features.Admin.ContentPages
{
    public class ContentPagesService : IContentPagesService
    {
        private const string ErrorDuplicateSlug = "Slug already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public ContentPagesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetContentPageDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.ContentPage
                .OrderBy(p => p.Title)
                .Select(p => new GetContentPageDTO(p.Id, p.Title, p.Slug))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetContentPageDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.ContentPage
                .Where(p => p.Id == id)
                .Select(p => new GetContentPageDTO(p.Id, p.Title, p.Slug))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ContentPagePublicDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _db.ContentPage
                .Where(p => p.Slug == slug)
                .Select(p => new ContentPagePublicDto(
                    p.Id,
                    p.Title,
                    p.Slug,
                    p.Blocks.Select(b => new ContentBlockPublicDto(
                        b.Slot,
                        b.Content
                    ))
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetContentPageDTO>> CreateAsync(CreateContentPageDTO request, CancellationToken cancellationToken = default)
        {
            var slugExists = await _db.ContentPage
                .AnyAsync(p => p.Slug == request.Slug, cancellationToken);
            if (slugExists)
                return ServiceResult<GetContentPageDTO>.Fail(ErrorDuplicateSlug);

            var page = new ContentPageEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = request.Slug
            };

            _db.ContentPage.Add(page);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetContentPageDTO>.Ok(new GetContentPageDTO(page.Id, page.Title, page.Slug));
        }

        public async Task<ServiceResult<GetContentPageDTO>> UpdateAsync(Guid id, UpdateContentPageDTO request, CancellationToken cancellationToken = default)
        {
            var page = await _db.ContentPage.FindAsync(new object?[] { id }, cancellationToken);
            if (page is null)
                return ServiceResult<GetContentPageDTO>.Fail(ErrorNotFound);

            page.Title = request.Title;
            page.Slug = request.Slug;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetContentPageDTO>.Ok(new GetContentPageDTO(page.Id, page.Title, page.Slug));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var page = await _db.ContentPage.FindAsync(new object?[] { id }, cancellationToken);
            if (page is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.ContentPage.Remove(page);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
