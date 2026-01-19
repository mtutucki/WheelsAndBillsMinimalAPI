using Microsoft.AspNetCore.Identity;
using WheelsAndBills.Domain.Entities.Auth;

namespace WheelsAndBillsAPI.Endpoints.Auth
{
    public static class RegisterEndpoint
    {
        public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/register", async (
                RegisterRequest request,
                UserManager<ApplicationUser> userManager) =>
            {
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                var result = await userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return Results.BadRequest(result.Errors);
                }

                return Results.Ok(new
                {
                    user.Id,
                    user.Email
                });
            });

            return app;
        }

        public record RegisterRequest(
            string Email,
            string Password,
            string FirstName,
            string LastName);
    }
}
