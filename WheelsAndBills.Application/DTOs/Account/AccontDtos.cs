namespace WheelsAndBills.Application.DTOs.Account.DTO
{
    public record GetMeDTO(
            Guid Id,
            string Name,
            string LastName,
            string Email,
            string? AvatarUrl,
            Guid? AvatarFileId,
            Guid? ExperienceLevelId,
            string? ExperienceLevelValue,
            DateTime? BirthDate,
            string? Country,
            string? City,
            DateTime CreatedAt,
            IReadOnlyList<string> Roles
     );

    public record UpdateMyProfileDTO(
        string FirstName,
        string LastName,
        Guid? AvatarFileId,
        Guid? ExperienceLevelId,
        DateTime? BirthDate,
        string? Country,
        string? City
    );

    public record ChangePasswordDTO(
        string CurrentPassword,
        string NewPassword
    );

    public record DeleteAccountDTO(
        string Password
        );

}
