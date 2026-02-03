using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WheelsAndBills.Application.DTOs.Account.DTO;
using WheelsAndBills.Application.Features.Account;

namespace WheelsAndBills.API.Endpoints.Account
{
    public static class UpdateMe
    {
        public static RouteHandlerBuilder MapUpdateMe(this RouteGroupBuilder app)
        {
            return app.MapPut("/me", [Authorize] async (
                UpdateMyProfileDTO request,
                ClaimsPrincipal user,
                IAccountService accountService,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var result = await accountService.UpdateMeAsync(userId, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.NoContent();
            });
        }
    }
}
