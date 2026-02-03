using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;

namespace WheelsAndBills.Application.Features.Admin.ContentBlocks
{
    public interface IContentBlocksService
    {
        Task<IReadOnlyList<GetContentBlockDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<GetContentBlockDTO>> GetByContentPageIdAsync(Guid contentPageId, CancellationToken cancellationToken = default);
        Task<ServiceResult<Guid>> CreateAsync(PostContentBlockDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetContentBlockDTO>> UpdateAsync(Guid id, PostContentBlockDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
