using Microsoft.AspNetCore.Identity;
using WheelsAndBills.Domain.Entities.Auth;
using WheelsAndBillsAPI.Endpoints.Auth.DTO;

namespace WheelsAndBillsAPI.Endpoints.Auth
{
    public static class RegisterEndpoint
    {
        public static RouteHandlerBuilder MapRegister(this RouteGroupBuilder app)
        {
            return app.MapPost("/register", async (
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
        }
    }
}
