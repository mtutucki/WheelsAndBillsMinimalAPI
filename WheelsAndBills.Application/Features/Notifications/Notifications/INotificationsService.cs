using WheelsAndBills.Application.Abstractions.Results;
using static WheelsAndBills.Application.DTOs.Notifications.NotificationsDTOs;

namespace WheelsAndBills.Application.Features.Notifications.Notifications
{
    public interface INotificationsService
    {
        Task<IReadOnlyList<GetNotificationDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetNotificationDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetNotificationDTO>> CreateAsync(CreateNotificationDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetNotificationDTO>> UpdateAsync(Guid id, UpdateNotificationDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
