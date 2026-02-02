namespace WheelsAndBillsAPI.Endpoints.Account.DTO
{
    public record GetMeDTO(
            Guid Id,
            string Name,
            string LastName,
            string Email,
            DateTime CreatedAt
     );

    public record UpdateMyProfileDTO(
        string FirstName,
        string LastName
    );
}
