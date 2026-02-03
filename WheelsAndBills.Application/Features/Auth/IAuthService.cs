using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Auth.DTO;

namespace WheelsAndBills.Application.Features.Auth
{
    public interface IAuthService
    {
        Task<ServiceResult<RegisterResult>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<ServiceResult<string>> LoginAsync(LoginDTO request, CancellationToken cancellationToken = default);
    }

    public record RegisterResult(Guid Id, string Email);
}
