using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Features.Dashboard;

namespace WheelsAndBills.API.BackgroundServices
{
    public class ServiceReminderNotificationsWorker : BackgroundService
    {
        private const int ReminderDaysThreshold = 30;
        private const int ReminderKmThreshold = 1000;
        private const int InsuranceReminderDaysThreshold = 30;
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServiceReminderNotificationsWorker> _logger;

        public ServiceReminderNotificationsWorker(
            IServiceProvider serviceProvider,
            ILogger<ServiceReminderNotificationsWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunOnceAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Service reminder worker failed.");
                }

                try
                {
                    await Task.Delay(Interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        private async Task RunOnceAsync(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var dashboard = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var serviceTypeId = await EnsureNotificationTypeAsync(db, "SERVICE_REMINDER", ct);
            var insuranceTypeId = await EnsureNotificationTypeAsync(db, "INSURANCE_EXPIRY", ct);

            if (serviceTypeId is null && insuranceTypeId is null)
                return;

            var userIds = await db.Users
                .Select(u => u.Id)
                .ToListAsync(ct);

            foreach (var userId in userIds)
            {
                var serviceEnabled = serviceTypeId.HasValue &&
                                     await IsNotificationEnabledAsync(db, userId, serviceTypeId.Value, ct);
                var insuranceEnabled = insuranceTypeId.HasValue &&
                                       await IsNotificationEnabledAsync(db, userId, insuranceTypeId.Value, ct);

                if (!serviceEnabled && !insuranceEnabled)
                    continue;

                if (serviceEnabled)
                {
                    var dashboardDto = await dashboard.GetForUserAsync(userId, ct);
                    if (dashboardDto is not null)
                    {
                        foreach (var reminder in dashboardDto.ServiceReminders)
                        {
                            if (!ShouldNotify(reminder))
                                continue;

                            var exists = await db.Notifications
                                .AnyAsync(n =>
                                    n.UserId == userId &&
                                    n.VehicleId == reminder.VehicleId &&
                                    n.NotificationTypeId == serviceTypeId &&
                                    n.ScheduledAt >= DateTime.UtcNow.Date,
                                    ct);
                            if (exists)
                                continue;

                            var (title, message) = BuildReminderMessage(reminder);

                            db.Notifications.Add(new WheelsAndBills.Domain.Entities.Notification.Notification
                            {
                                Id = Guid.NewGuid(),
                                UserId = userId,
                                VehicleId = reminder.VehicleId,
                                NotificationTypeId = serviceTypeId,
                                Title = title,
                                Message = message,
                                ScheduledAt = DateTime.UtcNow,
                                IsSent = false,
                                IsRead = false
                            });
                        }
                    }
                }

                if (insuranceEnabled && insuranceTypeId.HasValue)
                {
                    await CreateInsuranceNotificationsAsync(db, userId, insuranceTypeId.Value, ct);
                }
            }

            await db.SaveChangesAsync(ct);
        }

        private static bool ShouldNotify(WheelsAndBills.Application.DTOs.Dashboard.ServiceReminderDto reminder)
        {
            if (reminder.IsOverdueDate || reminder.IsOverdueMileage)
                return true;

            var nearByDate = reminder.NextServiceDate.HasValue &&
                             reminder.NextServiceDate.Value.Date <= DateTime.UtcNow.Date.AddDays(ReminderDaysThreshold) &&
                             reminder.NextServiceDate.Value.Date >= DateTime.UtcNow.Date;

            var nearByMileage = reminder.NextServiceMileage.HasValue &&
                                reminder.CurrentMileage.HasValue &&
                                reminder.CurrentMileage.Value >= reminder.NextServiceMileage.Value - ReminderKmThreshold;

            return nearByDate || nearByMileage;
        }

        private static async Task CreateInsuranceNotificationsAsync(
            IAppDbContext db,
            Guid userId,
            Guid typeId,
            CancellationToken ct)
        {
            var now = DateTime.UtcNow.Date;
            var threshold = now.AddDays(InsuranceReminderDaysThreshold);

            var vehicles = await db.Vehicles
                .Where(v => v.UserId == userId && v.InsuranceExpiryDate.HasValue)
                .Select(v => new
                {
                    v.Id,
                    Brand = v.Brand.Name,
                    Model = v.Model.Name,
                    v.Year,
                    Expiry = v.InsuranceExpiryDate!.Value.Date
                })
                .ToListAsync(ct);

            foreach (var v in vehicles)
            {
                if (v.Expiry > threshold)
                    continue;

                var exists = await db.Notifications
                    .AnyAsync(n =>
                        n.UserId == userId &&
                        n.VehicleId == v.Id &&
                        n.NotificationTypeId == typeId &&
                        n.ScheduledAt >= now,
                        ct);
                if (exists)
                    continue;

                var overdue = v.Expiry < now;
                var title = "Koniec polisy";
                var message = overdue
                    ? $"Polisa dla pojazdu \"{v.Brand} {v.Model} ({v.Year})\" wygasła {v.Expiry:yyyy-MM-dd}."
                    : $"Polisa dla pojazdu \"{v.Brand} {v.Model} ({v.Year})\" wygasa {v.Expiry:yyyy-MM-dd}.";

                db.Notifications.Add(new WheelsAndBills.Domain.Entities.Notification.Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    VehicleId = v.Id,
                    NotificationTypeId = typeId,
                    Title = title,
                    Message = message,
                    ScheduledAt = DateTime.UtcNow,
                    IsSent = false,
                    IsRead = false
                });
            }
        }

        private static (string Title, string Message) BuildReminderMessage(
            WheelsAndBills.Application.DTOs.Dashboard.ServiceReminderDto reminder)
        {
            if (reminder.IsOverdueDate || reminder.IsOverdueMileage)
            {
                return (
                    "Przypomnienie serwisowe",
                    $"Termin serwisu dla pojazdu \"{reminder.VehicleName}\" minął."
                );
            }

            var parts = new List<string>();
            if (reminder.NextServiceDate.HasValue)
                parts.Add($"data: {reminder.NextServiceDate:yyyy-MM-dd}");
            if (reminder.NextServiceMileage.HasValue)
                parts.Add($"przebieg: {reminder.NextServiceMileage} km");

            var suffix = parts.Count > 0 ? $" ({string.Join(", ", parts)})" : string.Empty;
            return (
                "Przypomnienie serwisowe",
                $"Zbliża się termin serwisu dla pojazdu \"{reminder.VehicleName}\"{suffix}."
            );
        }

        private static async Task<Guid?> EnsureNotificationTypeAsync(
            IAppDbContext db,
            string code,
            CancellationToken ct)
        {
            var existing = await db.NotificationTypes
                .Where(t => t.Code == code)
                .Select(t => new { t.Id })
                .FirstOrDefaultAsync(ct);

            if (existing is not null)
                return existing.Id;

            var type = new WheelsAndBills.Domain.Entities.Notification.NotificationType
            {
                Id = Guid.NewGuid(),
                Code = code
            };

            db.NotificationTypes.Add(type);
            await db.SaveChangesAsync(ct);

            return type.Id;
        }

        private static async Task<bool> IsNotificationEnabledAsync(
            IAppDbContext db,
            Guid userId,
            Guid notificationTypeId,
            CancellationToken ct)
        {
            var pref = await db.NotificationPreferences
                .Where(p => p.UserId == userId && p.NotificationTypeId == notificationTypeId)
                .Select(p => (bool?)p.IsEnabled)
                .FirstOrDefaultAsync(ct);

            return pref ?? true;
        }
    }
}
