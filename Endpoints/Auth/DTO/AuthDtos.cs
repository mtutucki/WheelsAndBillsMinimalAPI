namespace WheelsAndBillsAPI.Endpoints.Auth.DTO
{
    public record LoginDTO(string Email, string Password);
    public record RegisterRequest(
            string Email,
            string Password,
            string FirstName,
            string LastName);
}
