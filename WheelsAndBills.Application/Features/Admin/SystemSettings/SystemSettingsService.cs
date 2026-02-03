using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;
using SystemSettingEntity = WheelsAndBills.Domain.Entities.Admin.SystemSetting;

namespace WheelsAndBills.Application.Features.Admin.SystemSettings
{
    public class SystemSettingsService : ISystemSettingsService
    {
        private const string ErrorDuplicate = "System setting with this key already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public SystemSettingsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetSystemSettingDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.SystemSettings
                .OrderBy(s => s.Key)
                .Select(s => new GetSystemSettingDTO(s.Id, s.Key, s.Value))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetSystemSettingDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.SystemSettings
                .Where(s => s.Id == id)
                .Select(s => new GetSystemSettingDTO(s.Id, s.Key, s.Value))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<GetSystemSettingDTO?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _db.SystemSettings
                .Where(s => s.Key == key)
                .Select(s => new GetSystemSettingDTO(s.Id, s.Key, s.Value))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetSystemSettingDTO>> CreateAsync(CreateSystemSettingDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.SystemSettings
                .AnyAsync(s => s.Key == request.Key, cancellationToken);
            if (exists)
                return ServiceResult<GetSystemSettingDTO>.Fail(ErrorDuplicate);

            var setting = new SystemSettingEntity
            {
                Id = Guid.NewGuid(),
                Key = request.Key,
                Value = request.Value
            };

            _db.SystemSettings.Add(setting);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetSystemSettingDTO>.Ok(new GetSystemSettingDTO(setting.Id, setting.Key, setting.Value));
        }

        public async Task<ServiceResult<GetSystemSettingDTO>> UpdateAsync(Guid id, UpdateSystemSettingDTO request, CancellationToken cancellationToken = default)
        {
            var setting = await _db.SystemSettings.FindAsync(new object?[] { id }, cancellationToken);
            if (setting is null)
                return ServiceResult<GetSystemSettingDTO>.Fail(ErrorNotFound);

            setting.Value = request.Value;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetSystemSettingDTO>.Ok(new GetSystemSettingDTO(setting.Id, setting.Key, setting.Value));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var setting = await _db.SystemSettings.FindAsync(new object?[] { id }, cancellationToken);
            if (setting is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.SystemSettings.Remove(setting);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
