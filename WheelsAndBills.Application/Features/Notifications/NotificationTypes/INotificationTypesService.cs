using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Notifications.NotificationsDTOs;

namespace WheelsAndBills.Application.Features.Notifications.NotificationTypes
{
    public interface INotificationTypesService
    {
        Task<IReadOnlyList<GetNotificationTypeDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetNotificationTypeDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetNotificationTypeDTO>> CreateAsync(CreateNotificationTypeDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetNotificationTypeDTO>> UpdateAsync(Guid id, UpdateNotificationTypeDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
