using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;

namespace WheelsAndBills.Application.Features.Admin.FileResources
{
    public interface IFileResourcesService
    {
        Task<IReadOnlyList<GetFileResourceDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetFileResourceDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetFileResourceDTO>> CreateAsync(CreateFileResourceDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetFileResourceDTO>> UpdateAsync(Guid id, UpdateFileResourceDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
