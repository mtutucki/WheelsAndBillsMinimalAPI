using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Account.DTO;

namespace WheelsAndBills.Application.Features.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAppDbContext _db;

        public AccountService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<GetMeDTO?> GetMeAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => new GetMeDTO(
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email!,
                    u.CreatedAt
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult> UpdateMeAsync(Guid userId, UpdateMyProfileDTO request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                return ServiceResult.Fail("Imię i nazwisko są wymagane");
            }

            var entity = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (entity is null)
                return ServiceResult.Fail("NotFound");

            entity.FirstName = request.FirstName.Trim();
            entity.LastName = request.LastName.Trim();

            await _db.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
