using WheelsAndBills.Application.DTOs.Auth.DTO;
using WheelsAndBills.Application.Features.Auth;

namespace WheelsAndBills.API.Endpoints.Auth
{
    public static class Login
    {
        public static RouteHandlerBuilder MapLogin(this RouteGroupBuilder app)
        {
            return app.MapPost("/login", async (
                LoginDTO request,
                IAuthService authService,
                CancellationToken cancellationToken) =>
            {
                var result = await authService.LoginAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.Unauthorized();

                return Results.Ok(new { accessToken = result.Data });
            });
        }
    }
}
