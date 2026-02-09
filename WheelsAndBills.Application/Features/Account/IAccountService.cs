using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Account.DTO;

namespace WheelsAndBills.Application.Features.Account
{
    public interface IAccountService
    {
        Task<GetMeDTO?> GetMeAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ServiceResult> UpdateMeAsync(Guid userId, UpdateMyProfileDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> ChangePasswordAsync(Guid userId, ChangePasswordDTO request, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAccountAsync(Guid userId, DeleteAccountDTO request, CancellationToken cancellationToken = default);

    }
}
