using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using static WheelsAndBills.Application.DTOs.Notifications.NotificationsDTOs;

namespace WheelsAndBills.Application.Features.Notifications.Preferences
{
    public class NotificationPreferencesService : INotificationPreferencesService
    {
        private readonly IAppDbContext _db;

        public NotificationPreferencesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<NotificationPreferenceDTO>> GetForUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var types = await _db.NotificationTypes
                .Select(t => new { t.Id, t.Code })
                .ToListAsync(cancellationToken);

            var prefs = await _db.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);

            var prefMap = prefs.ToDictionary(p => p.NotificationTypeId, p => p.IsEnabled);

            return types.Select(t => new NotificationPreferenceDTO(
                t.Id,
                t.Code,
                prefMap.TryGetValue(t.Id, out var enabled) ? enabled : true
            )).ToList();
        }

        public async Task UpdateForUserAsync(
            Guid userId,
            UpdateNotificationPreferencesDTO request,
            CancellationToken cancellationToken = default)
        {
            var existing = await _db.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);

            var existingMap = existing.ToDictionary(p => p.NotificationTypeId, p => p);

            foreach (var pref in request.Preferences)
            {
                if (existingMap.TryGetValue(pref.NotificationTypeId, out var entity))
                {
                    entity.IsEnabled = pref.IsEnabled;
                }
                else
                {
                    _db.NotificationPreferences.Add(new Domain.Entities.Notification.NotificationPreference
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        NotificationTypeId = pref.NotificationTypeId,
                        IsEnabled = pref.IsEnabled
                    });
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
