using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using NotificationEntity = WheelsAndBills.Domain.Entities.Notification.Notification;
using static WheelsAndBills.Application.DTOs.Notifications.NotificationsDTOs;

namespace WheelsAndBills.Application.Features.Notifications.Notifications
{
    public class NotificationsService : INotificationsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorUserMissing = "User does not exist";
        private const string ErrorVehicleMissing = "Vehicle does not exist";

        private readonly IAppDbContext _db;

        public NotificationsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetNotificationDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Notifications
                .Select(n => new GetNotificationDTO(
                    n.Id,
                    n.UserId,
                    n.VehicleId,
                    n.NotificationTypeId,
                    n.NotificationType != null ? n.NotificationType.Code : null,
                    n.Title,
                    n.Message,
                    n.ScheduledAt,
                    n.IsSent,
                    n.IsRead
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<GetNotificationDTO>> GetForUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.ScheduledAt)
                .Select(n => new GetNotificationDTO(
                    n.Id,
                    n.UserId,
                    n.VehicleId,
                    n.NotificationTypeId,
                    n.NotificationType != null ? n.NotificationType.Code : null,
                    n.Title,
                    n.Message,
                    n.ScheduledAt,
                    n.IsSent,
                    n.IsRead
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetNotificationDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Notifications
                .Where(n => n.Id == id)
                .Select(n => new GetNotificationDTO(
                    n.Id,
                    n.UserId,
                    n.VehicleId,
                    n.NotificationTypeId,
                    n.NotificationType != null ? n.NotificationType.Code : null,
                    n.Title,
                    n.Message,
                    n.ScheduledAt,
                    n.IsSent,
                    n.IsRead
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetNotificationDTO>> CreateAsync(CreateNotificationDTO request, CancellationToken cancellationToken = default)
        {
            var userExists = await _db.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
            if (!userExists)
                return ServiceResult<GetNotificationDTO>.Fail(ErrorUserMissing);

            var vehicleExists = await _db.Vehicles.AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
            if (!vehicleExists)
                return ServiceResult<GetNotificationDTO>.Fail(ErrorVehicleMissing);

            var typeExists = await _db.NotificationTypes
                .AnyAsync(t => t.Id == request.NotificationTypeId, cancellationToken);
            if (!typeExists)
                return ServiceResult<GetNotificationDTO>.Fail("NotificationTypeMissing");

            var notification = new NotificationEntity
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                VehicleId = request.VehicleId,
                NotificationTypeId = request.NotificationTypeId,
                Title = request.Title,
                Message = request.Message,
                ScheduledAt = request.ScheduledAt,
                IsSent = false,
                IsRead = false
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetNotificationDTO>.Ok(new GetNotificationDTO(
                notification.Id,
                notification.UserId,
                notification.VehicleId,
                notification.NotificationTypeId,
                null,
                notification.Title,
                notification.Message,
                notification.ScheduledAt,
                notification.IsSent,
                notification.IsRead
            ));
        }

        public async Task<ServiceResult<GetNotificationDTO>> UpdateAsync(Guid id, UpdateNotificationDTO request, CancellationToken cancellationToken = default)
        {
            var notification = await _db.Notifications.FindAsync(new object?[] { id }, cancellationToken);
            if (notification is null)
                return ServiceResult<GetNotificationDTO>.Fail(ErrorNotFound);

            notification.Title = request.Title;
            notification.Message = request.Message;
            notification.ScheduledAt = request.ScheduledAt;
            notification.IsSent = request.IsSent;

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetNotificationDTO>.Ok(new GetNotificationDTO(
                notification.Id,
                notification.UserId,
                notification.VehicleId,
                notification.NotificationTypeId,
                null,
                notification.Title,
                notification.Message,
                notification.ScheduledAt,
                notification.IsSent,
                notification.IsRead
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var notification = await _db.Notifications.FindAsync(new object?[] { id }, cancellationToken);
            if (notification is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.Notifications.Remove(notification);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> MarkAsReadAsync(
            Guid id,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, cancellationToken);

            if (notification is null)
                return ServiceResult.Fail(ErrorNotFound);

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _db.SaveChangesAsync(cancellationToken);
            }

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> MarkAllAsReadAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            await _db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(n => n.IsRead, true),
                    cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
