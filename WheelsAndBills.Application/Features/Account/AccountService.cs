using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Account.DTO;
using WheelsAndBills.Domain.Entities.Auth;

namespace WheelsAndBills.Application.Features.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(IAppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<ServiceResult> ChangePasswordAsync(Guid userId, ChangePasswordDTO request, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(request.CurrentPassword) || String.IsNullOrWhiteSpace(request.NewPassword))
                return ServiceResult.Fail("Current and new password are required");

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return ServiceResult.Fail("User not found");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return ServiceResult.Fail(result.Errors.FirstOrDefault()?.Description ?? "Password change failed");

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAccountAsync(Guid userId, DeleteAccountDTO request, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(request.Password))
                return ServiceResult.Fail("Password is required");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return ServiceResult.Fail("User not found");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return ServiceResult.Fail("Invalid password");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return ServiceResult.Fail("Unable to delete user");

            return ServiceResult.Ok();
        }

        public async Task<GetMeDTO?> GetMeAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.AvatarUrl,
                    u.AvatarFileId,
                    u.ExperienceLevelId,
                    u.BirthDate,
                    u.Country,
                    u.City,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null) return null;

            string? experienceLevelValue = null;
            if (user.ExperienceLevelId.HasValue)
            {
                experienceLevelValue = await _db.DictionaryItems
                    .Where(i => i.Id == user.ExperienceLevelId.Value)
                    .Select(i => i.Value)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            string? avatarUrl = user.AvatarUrl;
            if (user.AvatarFileId.HasValue)
            {
                avatarUrl = await _db.FileResources
                    .Where(f => f.Id == user.AvatarFileId.Value)
                    .Select(f => f.FilePath)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var identityUser = await _userManager.FindByIdAsync(user.Id.ToString());
            var roles = identityUser is null
                ? new List<string>()
                : (await _userManager.GetRolesAsync(identityUser)).ToList();

            return new GetMeDTO(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                avatarUrl,
                user.AvatarFileId,
                user.ExperienceLevelId,
                experienceLevelValue,
                user.BirthDate,
                user.Country,
                user.City,
                user.CreatedAt,
                roles
            );
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
            if (request.AvatarFileId.HasValue)
            {
                var fileExists = await _db.FileResources.AnyAsync(
                    f => f.Id == request.AvatarFileId.Value,
                    cancellationToken);

                if (!fileExists)
                    return ServiceResult.Fail("AvatarFileNotFound");

                entity.AvatarFileId = request.AvatarFileId;
            }
            else
            {
                entity.AvatarFileId = null;
            }

            if (request.ExperienceLevelId is null)
            {
                entity.ExperienceLevelId = null;
            }
            else
            {
                var dictionaryId = await _db.Dictionaries
                    .Where(d => d.Code == "EXPERIENCE_LEVEL")
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (dictionaryId == Guid.Empty)
                    return ServiceResult.Fail("ExperienceLevelDictionaryNotFound");

                var exists = await _db.DictionaryItems.AnyAsync(
                    i => i.Id == request.ExperienceLevelId.Value && i.DictionaryId == dictionaryId,
                    cancellationToken);

                if (!exists)
                    return ServiceResult.Fail("InvalidExperienceLevel");

                entity.ExperienceLevelId = request.ExperienceLevelId;
            }

            entity.BirthDate = request.BirthDate;
            entity.Country = string.IsNullOrWhiteSpace(request.Country)
                ? null
                : request.Country.Trim();
            entity.City = string.IsNullOrWhiteSpace(request.City)
                ? null
                : request.City.Trim();

            await _db.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
