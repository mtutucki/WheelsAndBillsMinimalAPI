namespace WheelsAndBills.Application.DTOs.Account.DTO
{
    public record GetMeDTO(
            Guid Id,
            string Name,
            string LastName,
            string Email,
            DateTime CreatedAt,
            IReadOnlyList<string> Roles
     );

    public record UpdateMyProfileDTO(
        string FirstName,
        string LastName
    );

    public record ChangePasswordDTO(
        string CurrentPassword,
        string NewPassword
    );

    public record DeleteAccountDTO(
        string Password
        );

}
