using static WheelsAndBills.Application.DTOs.Notifications.NotificationsDTOs;

namespace WheelsAndBills.Application.Features.Notifications.Preferences
{
    public interface INotificationPreferencesService
    {
        Task<IReadOnlyList<NotificationPreferenceDTO>> GetForUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task UpdateForUserAsync(
            Guid userId,
            UpdateNotificationPreferencesDTO request,
            CancellationToken cancellationToken = default);
    }
}
