using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;

namespace WheelsAndBills.Application.Features.Admin.Dictionaries
{
    public interface IDictionariesService
    {
        Task<IReadOnlyList<GetDictionaryDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetDictionaryDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetDictionaryDTO>> CreateAsync(CreateDictionaryDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetDictionaryDTO>> UpdateAsync(Guid id, UpdateDictionaryDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
