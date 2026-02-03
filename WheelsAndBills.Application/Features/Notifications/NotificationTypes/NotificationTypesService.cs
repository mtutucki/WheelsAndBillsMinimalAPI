using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using NotificationTypeEntity = WheelsAndBills.Domain.Entities.Notification.NotificationType;
using static WheelsAndBills.Application.DTOs.Notifications.NotificationsDTOs;

namespace WheelsAndBills.Application.Features.Notifications.NotificationTypes
{
    public class NotificationTypesService : INotificationTypesService
    {
        private const string ErrorDuplicate = "NotificationType already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public NotificationTypesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetNotificationTypeDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.NotificationTypes
                .OrderBy(t => t.Code)
                .Select(t => new GetNotificationTypeDTO(t.Id, t.Code))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetNotificationTypeDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.NotificationTypes
                .Where(t => t.Id == id)
                .Select(t => new GetNotificationTypeDTO(t.Id, t.Code))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetNotificationTypeDTO>> CreateAsync(CreateNotificationTypeDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.NotificationTypes
                .AnyAsync(t => t.Code == request.Code, cancellationToken);
            if (exists)
                return ServiceResult<GetNotificationTypeDTO>.Fail(ErrorDuplicate);

            var type = new NotificationTypeEntity
            {
                Id = Guid.NewGuid(),
                Code = request.Code
            };

            _db.NotificationTypes.Add(type);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetNotificationTypeDTO>.Ok(new GetNotificationTypeDTO(type.Id, type.Code));
        }

        public async Task<ServiceResult<GetNotificationTypeDTO>> UpdateAsync(Guid id, UpdateNotificationTypeDTO request, CancellationToken cancellationToken = default)
        {
            var type = await _db.NotificationTypes.FindAsync(new object?[] { id }, cancellationToken);
            if (type is null)
                return ServiceResult<GetNotificationTypeDTO>.Fail(ErrorNotFound);

            var exists = await _db.NotificationTypes
                .AnyAsync(t => t.Code == request.Code && t.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetNotificationTypeDTO>.Fail(ErrorDuplicate);

            type.Code = request.Code;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetNotificationTypeDTO>.Ok(new GetNotificationTypeDTO(type.Id, type.Code));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var type = await _db.NotificationTypes.FindAsync(new object?[] { id }, cancellationToken);
            if (type is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.NotificationTypes.Remove(type);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
