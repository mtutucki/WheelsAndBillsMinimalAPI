using System.Security.Claims;
using WheelsAndBills.Application.Features.Account;

namespace WheelsAndBills.API.Endpoints.Account
{
    public static class ChangePassword
    {
        public static RouteHandlerBuilder MapChangePassword(this RouteGroupBuilder app)
        {
            return app.MapPut("/change-password", async (
                ChangePasswordDTO changePasswordDTO,
                ClaimsPrincipal user,
                IAccountService accountService,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var result = await accountService.ChangePasswordAsync(userId, changePasswordDTO, cancellationToken);

                if (!result.Success)
                    return result.Error == "NotFound" ? Results.NotFound() : Results.BadRequest(result.Error);

                return Results.NoContent();
            });
        }
    }
}
