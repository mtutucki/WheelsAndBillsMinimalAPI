using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;
using ContentBlockEntity = WheelsAndBills.Domain.Entities.Admin.ContentBlock;

namespace WheelsAndBills.Application.Features.Admin.ContentBlocks
{
    public class ContentBlocksService : IContentBlocksService
    {
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public ContentBlocksService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetContentBlockDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.ContentBlocks
                .OrderBy(b => b.Slot)
                .Select(b => new GetContentBlockDTO(
                    b.Id,
                    b.ContentPageId,
                    b.Content,
                    b.Slot
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<GetContentBlockDTO>> GetByContentPageIdAsync(Guid contentPageId, CancellationToken cancellationToken = default)
        {
            return await _db.ContentBlocks
                .Where(b => b.ContentPageId == contentPageId)
                .OrderBy(b => b.Slot)
                .Select(b => new GetContentBlockDTO(
                    b.Id,
                    b.ContentPageId,
                    b.Content,
                    b.Slot
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<ServiceResult<Guid>> CreateAsync(PostContentBlockDTO request, CancellationToken cancellationToken = default)
        {
            var block = new ContentBlockEntity
            {
                Id = Guid.NewGuid(),
                ContentPageId = request.ContentPageId,
                Content = request.Content,
                Slot = request.Slot
            };

            _db.ContentBlocks.Add(block);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<Guid>.Ok(block.Id);
        }

        public async Task<ServiceResult<GetContentBlockDTO>> UpdateAsync(Guid id, PostContentBlockDTO request, CancellationToken cancellationToken = default)
        {
            var block = await _db.ContentBlocks.FindAsync(new object?[] { id }, cancellationToken);
            if (block is null)
                return ServiceResult<GetContentBlockDTO>.Fail(ErrorNotFound);

            block.ContentPageId = request.ContentPageId;
            block.Slot = request.Slot;
            block.Content = request.Content;

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetContentBlockDTO>.Ok(new GetContentBlockDTO(
                block.Id,
                block.ContentPageId,
                block.Content,
                block.Slot
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var block = await _db.ContentBlocks.FindAsync(new object?[] { id }, cancellationToken);
            if (block is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.ContentBlocks.Remove(block);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
