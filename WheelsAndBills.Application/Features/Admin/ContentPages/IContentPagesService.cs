using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;

namespace WheelsAndBills.Application.Features.Admin.ContentPages
{
    public interface IContentPagesService
    {
        Task<IReadOnlyList<GetContentPageDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetContentPageDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ContentPagePublicDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetContentPageDTO>> CreateAsync(CreateContentPageDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetContentPageDTO>> UpdateAsync(Guid id, UpdateContentPageDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
