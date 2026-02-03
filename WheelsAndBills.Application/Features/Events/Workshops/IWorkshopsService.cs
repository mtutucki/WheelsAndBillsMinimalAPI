using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Events.EventsDTO;

namespace WheelsAndBills.Application.Features.Events.Workshops
{
    public interface IWorkshopsService
    {
        Task<IReadOnlyList<GetWorkshopDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetWorkshopDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetWorkshopDTO>> CreateAsync(CreateWorkshopDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetWorkshopDTO>> UpdateAsync(Guid id, UpdateWorkshopDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
