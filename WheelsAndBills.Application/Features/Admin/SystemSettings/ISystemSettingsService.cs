using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Admin.DTO;

namespace WheelsAndBills.Application.Features.Admin.SystemSettings
{
    public interface ISystemSettingsService
    {
        Task<IReadOnlyList<GetSystemSettingDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GetSystemSettingDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GetSystemSettingDTO?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetSystemSettingDTO>> CreateAsync(CreateSystemSettingDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult<GetSystemSettingDTO>> UpdateAsync(Guid id, UpdateSystemSettingDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
