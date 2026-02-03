using WheelsAndBills.Application.DTOs.Auth.DTO;
using WheelsAndBills.Application.Features.Auth;

namespace WheelsAndBills.API.Endpoints.Auth
{
    public static class RegisterEndpoint
    {
        public static RouteHandlerBuilder MapRegister(this RouteGroupBuilder app)
        {
            return app.MapPost("/register", async (
                RegisterRequest request,
                IAuthService authService,
                CancellationToken cancellationToken) =>
            {
                var result = await authService.RegisterAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Ok(new { result.Data!.Id, result.Data.Email });
            });
        }
    }
}
