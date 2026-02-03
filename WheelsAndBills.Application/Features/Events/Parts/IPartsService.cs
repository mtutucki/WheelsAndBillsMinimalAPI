using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.Parts
{
    public interface IPartsService
    {
        Task<IReadOnlyList<GetPartDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetPartDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetPartDTO>> CreateAsync(CreatePartDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetPartDTO>> UpdateAsync(Guid id, UpdatePartDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
