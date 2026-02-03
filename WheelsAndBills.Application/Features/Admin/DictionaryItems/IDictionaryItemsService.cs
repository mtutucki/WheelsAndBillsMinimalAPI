using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;

namespace WheelsAndBills.Application.Features.Admin.DictionaryItems
{
    public interface IDictionaryItemsService
    {
        Task<IReadOnlyList<GetDictionaryItemDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<GetDictionaryItemDTO>> GetByDictionaryIdAsync(Guid dictionaryId, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetDictionaryItemDTO>> CreateAsync(CreateDictionaryItemDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetDictionaryItemDTO>> UpdateAsync(Guid id, UpdateDictionaryItemDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
